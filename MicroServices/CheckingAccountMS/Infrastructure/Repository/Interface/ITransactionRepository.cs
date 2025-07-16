using CheckingAccountMS.Domain.Entities;

namespace CheckingAccountMS.Infrastructure.Repository.Interface
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
