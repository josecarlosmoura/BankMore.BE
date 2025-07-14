using Domain.Enuns;

namespace Domain.Entities
{
    public class Transaction
    {
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();
        public string AccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }

        // Navigation property for related transactions
        public CheckingAccount Account { get; set; } = default!;
    }
}
