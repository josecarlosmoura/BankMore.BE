using System.Text.Json.Serialization;

namespace Domain.Enuns
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Credit,
        Debit
    }
}
