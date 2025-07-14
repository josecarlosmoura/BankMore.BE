using CheckingAccountMS.Application.DTOs;
using MediatR;

namespace CheckingAccountMS.Application.Queries.GetAccountBalance
{
    public class GetAccountBalanceQuery : IRequest<AccountBalanceDto?>
    {
        public GetAccountBalanceQuery()
        {
            // Empty constructor for MediatR
        }
    }
}
