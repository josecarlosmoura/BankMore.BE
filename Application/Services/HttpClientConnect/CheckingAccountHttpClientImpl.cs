using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransferMS.Application.DTOs;
using TransferMS.Application.Enuns;
using TransferMS.Application.Models;

namespace TransferMS.Application.Services.HttpClientConnect
{
    public class CheckingAccountHttpClient : ICheckingAccountHttpClient
    {
        private readonly HttpClient _httpClient;

        public CheckingAccountHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResult> DebitAsync(decimal amount, Guid IdempotencyKey, string token)
        {
            var transferDto = new TransactionDto
            {
                Amount = amount,
                AccountNumber = null, // Assuming account number is not needed for debit
                IdempotencyKey = IdempotencyKey,
                TransactionType = TransactionType.Debit
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "transfer")
            {
                Content = JsonContent.Create(transferDto)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();

            return result;
        }

        public async Task<OperationResult> CreditAsync(long? accountNumber, decimal amount, Guid IdempotencyKey, string token)
        {
            var transferDto = new TransactionDto
            {
                Amount = amount,
                AccountNumber = accountNumber,
                IdempotencyKey = IdempotencyKey,
                TransactionType = TransactionType.Credit
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "transfer")
            {
                Content = JsonContent.Create(transferDto)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();

            return result;
        }
    }
}
