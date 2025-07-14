namespace CheckingAccountMS.Application.DTOs
{
    public class AccountBalanceDto
    {
        public long AccountNumber { get; set; } = default!;
        public string AccountHolderName { get; set; } = default!;
        public DateTime QueriedAt { get; set; } = DateTime.UtcNow;
        public string Balance { get; set; } = "0,00"; // alterado para string
    }
}
