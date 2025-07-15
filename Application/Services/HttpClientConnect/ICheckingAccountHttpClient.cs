using TransferMS.Application.Models;

namespace TransferMS.Application.Services.HttpClientConnect
{
    public interface ICheckingAccountHttpClient
    {
        Task<bool> IsValidAsync(string accountId, string token);
        Task<bool> IsActiveAsync(string accountId, string token);
        Task<OperationResult> DebitAsync(string accountId, decimal amount, string requestId, string token);
        Task<OperationResult> CreditAsync(string accountNumber, decimal amount, string requestId, string token);
    }
}
