using Application.Exeption;
using Domain.Entities;
using Domain.Enuns;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Commands.CreateTransaction
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, string>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateTransactionCommandHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            // Verifica se a requisição já foi processada
            var existing = await _context.Idempotencies
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.IdempotencyKey.Equals(request.IdempotencyKey.ToString()), cancellationToken);


            if (existing != null) return existing.Result; // já foi executada

            CheckingAccount? account;

            if (request.AccountNumber != null)
            {
                account = await _context.CheckingAccounts.FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber, cancellationToken);
            }
            else
            {
                var accountId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(accountId))
                    throw new UnauthorizedAccessException("Invalid or expired token.");

                account = await _context.CheckingAccounts
                .FirstOrDefaultAsync(a => a.CheckingAccountId == accountId, cancellationToken);

                if (account == null)
                    throw new ServiceException(ServiceError.InvalidAccount);
            }

            if (!account.IsActive)
            {
                throw new ServiceException(ServiceError.InactiveAccount);
            }

            if (request.Amount <= 0)
            {
                throw new ServiceException(ServiceError.InvalidValue);
            }

            if (request.TransactionType != TransactionType.Debit && request.TransactionType != TransactionType.Credit)
            {
                throw new ServiceException(ServiceError.InvalidType);

            }

            if (request.AccountNumber != account.AccountNumber && request.TransactionType == TransactionType.Credit)
            {
                throw new ServiceException(ServiceError.InvalidType);
            }

            var transaction = new Transaction
            {
                AccountId = account.CheckingAccountId,
                Amount = request.Amount,
                TransactionType = request.TransactionType
            };

            _context.Transactions.Add(transaction);

            // Persistir a chave de idempotência
            _context.Idempotencies.Add(new Idempotency
            {
                IdempotencyKey = request.IdempotencyKey.ToString(),
                Result = JsonSerializer.Serialize(transaction)
            });
            await _context.SaveChangesAsync(cancellationToken);

            return transaction.TransactionId;
        }
    }
}
