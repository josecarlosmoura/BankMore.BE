using TransferMS.Domain.Entities;

namespace TransferMS.Infrastructure.Repository.Interface
{
    public interface ITransferRepository
    {
        Task AddAsync(Transfer transfer);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
