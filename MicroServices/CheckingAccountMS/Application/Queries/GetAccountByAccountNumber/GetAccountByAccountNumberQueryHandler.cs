using Application.DTOs;
using CheckingAccountMS.Infrastructure.Data;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CheckingAccountMS.Application.Queries.GetAccountByAccountNumber
{
    public class GetAccountByAccountNumberQueryHandler : IRequestHandler<GetAccountByAccountNumberQuery, AccountDto?>
    {
        private readonly ICheckingAccountRepository _checkingAccountRepository;

        public GetAccountByAccountNumberQueryHandler(ICheckingAccountRepository checkingAccountRepository)
        {
            _checkingAccountRepository = checkingAccountRepository;
        }

        public async Task<AccountDto?> Handle(GetAccountByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            var account = await _checkingAccountRepository.FirstOrDefaultAsync(c => c.AccountNumber == request.AccountNumber);

            return account == null ? null : new AccountDto(account);
        }
    }
}
