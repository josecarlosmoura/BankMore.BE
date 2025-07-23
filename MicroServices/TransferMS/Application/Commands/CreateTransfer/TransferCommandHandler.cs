using Application.DTOs;
using BuildingBlocks.Exeption;
using FluentResults;
using MediatR;
using System.Text.Json;
using TransferMS.Infrastructure.Repository.Interface;
using TransferMS.Application.Models;
using TransferMS.Application.Services.CheckingAccount;
using TransferMS.Application.Services.HttpClientConnect;
using TransferMS.Domain.Entities;
using TransferMS.Infrastructure.Repository.Interface;

namespace TransferMS.Application.Commands.CreateTransfer
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;
        private readonly ICheckingAccountService _checkingAccount;
        private readonly ICheckingAccountHttpClient _accountClient;

        public TransferCommandHandler(
            ITransferRepository transferRepository,
            IIdempotencyRepository idempotencyRepository,
            ICheckingAccountService checkingAccount,
            ICheckingAccountHttpClient accountClient
            )
        {
            _transferRepository = transferRepository;
            _idempotencyRepository = idempotencyRepository;
            _checkingAccount = checkingAccount;
            _accountClient = accountClient;
        }

        public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new ServiceException(ServiceError.InvalidValue);

            TransferCommand transferDebit = new TransferCommand 
            {
                Amount = request.Amount
            }
            ;
            var debit = await _accountClient.DebitAsync(transferDebit.Amount, transferDebit.IdempotencyKey, _checkingAccount.JwtToken);
            if (!debit.Success)
                throw new ServiceException(ServiceError.DebitFailed);

            TransferCommand? transferRefund = null;
            OperationResult? transferRefundResult = null;

            TransferCommand transferRequest = new TransferCommand { Amount = request.Amount, DestinationAccountNumber = request.DestinationAccountNumber };
            var credit = await _accountClient.CreditAsync(transferRequest.DestinationAccountNumber, transferRequest.Amount, transferRequest.IdempotencyKey, _checkingAccount.JwtToken);
            if (!credit.Success)
            {
                transferRefund = new TransferCommand { Amount = request.Amount, DestinationAccountNumber = request.DestinationAccountNumber };
                transferRefundResult = await _accountClient.CreditAsync(null, transferRefund.Amount, transferRefund.IdempotencyKey, _checkingAccount.JwtToken);
            }

            var transfer = new Transfer
            {
                TransferId = Guid.NewGuid().ToString(),
                IdCheckingAccountFrom = debit.AccountId,
                IdCheckingAccountTo = credit.AccountId,
                Amount = request.Amount,
                TransferDate = DateTime.UtcNow
            };

            await _transferRepository.AddAsync(transfer);
            await _transferRepository.SaveChangesAsync(cancellationToken);

            var resultToIdempotency = new ResultToIdempotencyDto
            {
                IdempotencyKeyFromDebit = transferDebit.IdempotencyKey,
                DebitResult = debit,
                IdempotencyKeyFromCredit = transferRequest.IdempotencyKey,
                CreditResult = credit,
                IdempotencyKeyFromTransferRefund = transferRefund?.IdempotencyKey,
                TransferRefundResult = transferRefundResult
            };

            // Persistir a chave de idempotência
            await _idempotencyRepository.AddAsync(new Idempotency
            {
                IdempotencyKey = request.IdempotencyKey.ToString(),
                Result = JsonSerializer.Serialize(resultToIdempotency)
            });
            await _idempotencyRepository.SaveChangesAsync(cancellationToken);
            
            return Result.Ok();
        }
    }
}
