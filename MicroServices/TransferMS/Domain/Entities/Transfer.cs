namespace TransferMS.Domain.Entities
{
    public class Transfer
    {
        public string TransferId { get; set; } = Guid.NewGuid().ToString();
        public string IdCheckingAccountFrom { get; set; } = default!;
        public string IdCheckingAccountTo { get; set; } = default!;
        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; } = 0m;
    }
}
