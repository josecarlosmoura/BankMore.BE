namespace CheckingAccountMS.Domain.Entities
{
    public class CheckingAccount
    {
        public string CheckingAccountId { get; set; } = Guid.NewGuid().ToString();

        public long AccountNumber { get; set; }

        public string Cpf { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public string Password { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        public decimal Balance { get; set; } = 0m;

        public CheckingAccount()
        {

        }
    }
}
