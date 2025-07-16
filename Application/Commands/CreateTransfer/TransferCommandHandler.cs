using Application.DTOs;
using BuildingBlocks.Exeption;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using TransferMS.Application.Models;
using TransferMS.Application.Services.CheckingAccount;
using TransferMS.Application.Services.HttpClientConnect;
using TransferMS.Domain.Entities;
using TransferMS.Infrastructure.Data;

namespace TransferMS.Application.Commands.CreateTransfer
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
    {
        private readonly ICheckingAccountService _checkingAccount;
        private readonly ICheckingAccountHttpClient _accountClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public TransferCommandHandler(
            ICheckingAccountService checkingAccount,
            ICheckingAccountHttpClient accountClient,
            IHttpContextAccessor httpContextAccessor,
            AppDbContext context)
        {
            _checkingAccount = checkingAccount;
            _accountClient = accountClient;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
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

            _context.Transfers.Add(transfer);

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
            _context.Idempotencies.Add(new Idempotency
            {
                IdempotencyKey = request.IdempotencyKey.ToString(),
                Result = JsonSerializer.Serialize(resultToIdempotency)
            });

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
