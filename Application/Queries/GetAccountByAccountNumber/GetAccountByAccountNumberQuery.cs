using Application.DTOs;
using MediatR;

namespace Application.Queries.GetAccountById
{
    public class GetAccountByAccountNumberQuery : IRequest<AccountDto?>
    {
        public long AccountNumber { get; set; }

        public GetAccountByAccountNumberQuery(long accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }
}
