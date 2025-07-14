using Application.Services.CheckingAccount;
using BuildingBlocks.Exeption;
using CheckingAccountMS.Application.DTOs;
using CheckingAccountMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CheckingAccountMS.Application.Queries.GetAccountBalance
{
    public class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, AccountBalanceDto>
    {
        private readonly AppDbContext _context;
        private readonly ICheckingAccountService _checkingAccountService;

        public GetAccountBalanceQueryHandler(AppDbContext context, ICheckingAccountService checkingAccountService)
        {
            _context = context;
            _checkingAccountService = checkingAccountService;
        }

        public async Task<AccountBalanceDto> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
        {
            var accountId = _checkingAccountService.GetAccountId();

            var account = await _context.CheckingAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountNumber == accountId);

            if (string.IsNullOrEmpty(account.CheckingAccountId))
                throw new ServiceException(ServiceError.InvalidAccount);

            if (!account.IsActive)
            {
                throw new ServiceException(ServiceError.InactiveAccount);
            }

            return new AccountBalanceDto
            {
                AccountNumber = account.AccountNumber,
                AccountHolderName = account.FullName,
                QueriedAt = DateTime.UtcNow,
                Balance = account.Balance.ToString("N2", System.Globalization.CultureInfo.InvariantCulture) // Formata o saldo como string com duas casas decimais
            };
        }
    }
}
