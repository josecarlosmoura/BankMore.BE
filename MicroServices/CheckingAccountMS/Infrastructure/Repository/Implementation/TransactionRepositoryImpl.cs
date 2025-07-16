using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Infrastructure.Data;
using CheckingAccountMS.Infrastructure.Repository.Interface;

namespace CheckingAccountMS.Infrastructure.Repository.Implementation
{
    public class TransactionRepositoryImpl : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
