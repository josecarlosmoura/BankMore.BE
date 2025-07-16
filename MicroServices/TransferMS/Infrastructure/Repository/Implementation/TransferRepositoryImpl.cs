using TransferMS.Domain.Entities;
using TransferMS.Infrastructure.Data;
using TransferMD.Infrastructure.Repository.Interface;

namespace TransferMS.Infrastructure.Repository.Implementation
{
    public class TransferRepositoryImpl : ITransferRepository
    {
        private readonly AppDbContext _context;

        public TransferRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transfer transfer)
        {
            await _context.Transfers.AddAsync(transfer);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
