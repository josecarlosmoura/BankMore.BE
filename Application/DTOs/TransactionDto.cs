using TransferMS.Application.Enuns;

namespace TransferMS.Application.DTOs
{
    public class TransactionDto
    {
        public Guid IdempotencyKey { get; set; }
        public long? AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
