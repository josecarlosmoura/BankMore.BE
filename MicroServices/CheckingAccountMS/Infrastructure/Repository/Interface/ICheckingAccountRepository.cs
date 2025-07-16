using CheckingAccountMS.Domain.Entities;
using System.Linq.Expressions;

namespace CheckingAccountMS.Infrastructure.Repository.Interface
{
    public interface ICheckingAccountRepository
    {
        Task AddAsync(CheckingAccount checkingAccount);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<CheckingAccount?> FirstOrDefaultAsync(Expression<Func<CheckingAccount, bool>> predicate);
    }
}
