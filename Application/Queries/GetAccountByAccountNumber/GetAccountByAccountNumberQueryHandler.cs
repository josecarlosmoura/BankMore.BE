using Application.DTOs;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.GetAccountByAccountNumber
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
            var account = await _context.ContaCorrente
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Numero == request.AccountNumber, cancellationToken);

            return account == null ? null : new AccountDto(account);
        }
    }
}
