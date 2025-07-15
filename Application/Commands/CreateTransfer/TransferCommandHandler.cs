using BuildingBlocks.Exeption;
using FluentResults;
using MediatR;
using TransferMS.Application.Services.CheckingAccount;
using TransferMS.Application.Services.HttpClientConnect;
using TransferMS.Domain.Entities;
using TransferMS.Infrastructure.Data;

namespace TransferMS.Application.Commands.CreateTransfer
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
    {
        private readonly ICheckingAccountService _currentUser;
        private readonly ICheckingAccountHttpClient _accountClient;
        private readonly AppDbContext _context;

        public TransferCommandHandler(
            ICheckingAccountService currentUser,
            ICheckingAccountHttpClient accountClient,
            AppDbContext context)
        {
            _currentUser = currentUser;
            _accountClient = accountClient;
            _context = context;
        }

        public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new ServiceException(ServiceError.InvalidValue);

            if (!await _accountClient.IsValidAsync(_currentUser.AccountId, _currentUser.JwtToken))
                throw new ServiceException(ServiceError.InvalidAccount);

            if (!await _accountClient.IsActiveAsync(_currentUser.AccountId, _currentUser.JwtToken))
                throw new ServiceException(ServiceError.InactiveAccount);

            var debit = await _accountClient.DebitAsync(_currentUser.AccountId, request.Amount, request.RequestId, _currentUser.JwtToken);
            if (!debit.Success)
                throw new ServiceException(ServiceError.DebitFailed);

            var credit = await _accountClient.CreditAsync(request.DestinationAccountNumber, request.Amount, request.RequestId, _currentUser.JwtToken);
            if (!credit.Success)
            {
                await _accountClient.CreditAsync(_currentUser.AccountId, request.Amount, request.RequestId, _currentUser.JwtToken);
                throw new ServiceException(ServiceError.CreditFailed);
            }

            var transfer = new Transfer
            {
                TransferId = Guid.NewGuid().ToString(),
                IdCheckingAccountFrom = _currentUser.AccountId,
                IdCheckingAccountTo = credit.AccountId,
                Amount = request.Amount,
                TransferDate = DateTime.UtcNow
            };

            _context.Transfers.Add(transfer);
            await _context.AddAsync(transfer);
            return Result.Ok();
        }
    }
}
