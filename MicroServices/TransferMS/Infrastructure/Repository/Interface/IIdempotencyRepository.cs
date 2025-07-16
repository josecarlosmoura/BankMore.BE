using System.Linq.Expressions;
using TransferMS.Domain.Entities;

namespace TransferMS.Infrastructure.Repository.Interface
{
    public interface IIdempotencyRepository
    {
        Task AddAsync(Idempotency idempotency);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<Idempotency?> FirstOrDefaultNoTrackingAsync(Expression<Func<Idempotency, bool>> predicate, CancellationToken cancellationToken);
    }
}
