using CheckingAccountMS.Domain.Entities;

namespace Application.DTOs
{
    public class AccountDto
    {
        public long AccountNumber { get; set; }

        public AccountDto(CheckingAccount account)
        {
            AccountNumber = account.AccountNumber;
        }
    }
}
