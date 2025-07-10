using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
