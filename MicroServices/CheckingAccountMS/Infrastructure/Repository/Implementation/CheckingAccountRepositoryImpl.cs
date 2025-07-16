using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Infrastructure.Data;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CheckingAccountMS.Infrastructure.Repository.Implementation
{
    public class CheckingAccountRepositoryImpl : ICheckingAccountRepository
    {
        private readonly AppDbContext _context;

        public CheckingAccountRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CheckingAccount checkingAccount)
        {
            await _context.AddAsync(checkingAccount);
        }

        public async Task<CheckingAccount?> FirstOrDefaultAsync(Expression<Func<CheckingAccount, bool>> predicate)
        {
            return await _context.CheckingAccounts.FirstOrDefaultAsync(predicate);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
