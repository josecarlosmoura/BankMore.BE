using BuildingBlocks.Exeption;
using CheckingAccountMS.Application.Commands.CreateTransfer;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Domain.Enuns;
using CheckingAccountMS.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace CheckingAccountMS.Application.Commands.CreateTransfer
{
    public class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand, string>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateTransferCommandHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
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

                if (account == null)
                    throw new ServiceException(ServiceError.InvalidAccount);
            }
            else
            {
                var accountId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

            var transaction = new Transaction
            {
                AccountId = account.CheckingAccountId,
                Amount = request.Amount,
                TransactionType = request.TransactionType
            };

            // Atualiza saldo na conta
            if (transaction.TransactionType == TransactionType.Credit)
                account.Balance += transaction.Amount;
            else
            {
                if (account.Balance < transaction.Amount)
                    throw new ServiceException(ServiceError.InsufficientBalance);

                account.Balance -= transaction.Amount;
            }           

            _context.Transactions.Add(transaction);

            // Persistir a chave de idempotência
            _context.Idempotencies.Add(new Idempotency
            {
                IdempotencyKey = request.IdempotencyKey.ToString(),
                Result = JsonSerializer.Serialize(transaction)
            });
            await _context.SaveChangesAsync(cancellationToken);

            return transaction.AccountId;
        }
    }
}
