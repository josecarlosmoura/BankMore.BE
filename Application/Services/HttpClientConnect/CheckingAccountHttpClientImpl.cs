using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        // Todo : Implementar um único endpoint lincando no endpoint de transações, pois ele já faz todas as validações necessárias.

        public async Task<bool> IsValidAsync(string accountId, string token)
        {
            // Simulado. Implemente a chamada real conforme seu endpoint de conta.
            return true;
        }

        public async Task<bool> IsActiveAsync(string accountId, string token)
        {
            // Simulado. Implemente a chamada real conforme seu endpoint de conta.
            return true;
        }

        public async Task<OperationResult> DebitAsync(string accountId, decimal amount, string requestId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/checking-account/debit")
            {
                Content = JsonContent.Create(new { accountId, amount, requestId })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            return new OperationResult { Success = response.IsSuccessStatusCode };
        }

        public async Task<OperationResult> CreditAsync(string accountNumber, decimal amount, string requestId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/checking-account/credit")
            {
                Content = JsonContent.Create(new { accountNumber, amount, requestId })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            return new OperationResult { Success = response.IsSuccessStatusCode };
        }
    }
}
