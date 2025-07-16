using CheckingAccountMS.Domain.Entities;
using System.Linq.Expressions;

namespace CheckingAccountMS.Infrastructure.Repository.Interface
{
    public interface IIdempotencyRepository
    {
        Task AddAsync(Idempotency idempotency);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<Idempotency?> FirstOrDefaultNoTrackingAsync(Expression<Func<Idempotency, bool>> predicate, CancellationToken cancellationToken);
    }
}
