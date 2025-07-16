using System.Text.Json.Serialization;

namespace TransferMS.Domain.Enuns
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Credit,
        Debit
    }
}
