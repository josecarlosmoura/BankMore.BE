using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Infrastructure.Data;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CheckingAccountMS.Infrastructure.Repository.Implementation
{
    public class IdempotencyRepositoryImpl : IIdempotencyRepository
    {
        private readonly AppDbContext _context;

        public IdempotencyRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Idempotency idempotency)
        {
            await _context.Idempotencies.AddAsync(idempotency);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Idempotency?> FirstOrDefaultNoTrackingAsync(Expression<Func<Idempotency, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Idempotencies.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }
    }
}
