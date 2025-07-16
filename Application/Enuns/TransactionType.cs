using System.Text.Json.Serialization;

namespace TransferMS.Application.Enuns
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Credit,
        Debit
    }
}
