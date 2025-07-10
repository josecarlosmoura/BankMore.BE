using Domain.Entities;

namespace Application.DTOs
{
    public class AccountDto
    {
        public long AccountNumber { get; set; }

        public AccountDto(ContaCorrente account)
        {
            AccountNumber = account.Numero;
        }
    }
}
