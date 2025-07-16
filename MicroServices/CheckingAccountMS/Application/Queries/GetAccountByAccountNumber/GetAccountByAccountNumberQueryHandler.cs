using Application.DTOs;
using CheckingAccountMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CheckingAccountMS.Application.Queries.GetAccountByAccountNumber
{
    public class GetAccountByAccountNumberQueryHandler : IRequestHandler<GetAccountByAccountNumberQuery, AccountDto?>
    {
        private readonly AppDbContext _context;

        public GetAccountByAccountNumberQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AccountDto?> Handle(GetAccountByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.CheckingAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.AccountNumber == request.AccountNumber, cancellationToken);

            return account == null ? null : new AccountDto(account);
        }
    }
}
