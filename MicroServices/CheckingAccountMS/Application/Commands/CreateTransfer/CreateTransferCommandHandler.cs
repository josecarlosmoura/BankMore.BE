using BuildingBlocks.Exeption;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Domain.Enuns;
using CheckingAccountMS.Infrastructure.Data;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace CheckingAccountMS.Application.Commands.CreateTransfer
{
    public class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand, string>
    {        
        private readonly ICheckingAccountRepository _checkingAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateTransferCommandHandler(ICheckingAccountRepository checkingAccountRepository,
            ITransactionRepository transactionRepository,
            IIdempotencyRepository idempotencyRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _checkingAccountRepository = checkingAccountRepository;
            _transactionRepository = transactionRepository;
            _idempotencyRepository = idempotencyRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
        {
            // Verifica se a requisição já foi processada
            var existing = await _idempotencyRepository.FirstOrDefaultNoTrackingAsync(i => i.IdempotencyKey.Equals(request.IdempotencyKey.ToString()), cancellationToken);

            if (existing != null) return existing.Result; // já foi executada

            CheckingAccount? account;

            if (request.AccountNumber != null)
            {
                account = await _checkingAccountRepository.FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);

                if (account == null)
                    throw new ServiceException(ServiceError.InvalidAccount);
            }
            else
            {
                var accountId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                account = await _checkingAccountRepository.FirstOrDefaultAsync(a => a.CheckingAccountId == accountId);

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

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync(cancellationToken);

            // Persistir a idempotência
            await _idempotencyRepository.AddAsync(new Idempotency
            {
                IdempotencyKey = request.IdempotencyKey.ToString(),
                Result = JsonSerializer.Serialize(transaction)
            });
            await _idempotencyRepository.SaveChangesAsync(cancellationToken);


            return transaction.AccountId;
        }
    }
}
