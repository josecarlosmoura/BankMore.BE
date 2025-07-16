using CheckingAccountMS.Domain.Entities;

namespace CheckingAccountMS.Infrastructure.Repository.Interface
{
    public interface IIdempotencyRepository
    {
        Task AddAsync(Idempotency idempotency);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
