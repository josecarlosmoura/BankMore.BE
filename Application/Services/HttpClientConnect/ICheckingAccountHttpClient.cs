using TransferMS.Application.Models;

namespace TransferMS.Application.Services.HttpClientConnect
{
    public interface ICheckingAccountHttpClient
    {
        Task<OperationResult> DebitAsync(decimal amount, Guid IdempotencyKey, string token);
        Task<OperationResult> CreditAsync(long? accountNumber, decimal amount, Guid IdempotencyKey, string token);
    }
}
