using System.Text.Json.Serialization;

namespace CheckingAccountMS.Domain.Enuns
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Credit,
        Debit
    }
}
