using Application.DTOs;
using MediatR;

namespace Application.Queries.GetAccountBalance
{
    public class GetAccountBalanceQuery : IRequest<AccountBalanceDto?>
    {
        public GetAccountBalanceQuery()
        {
            // Empty constructor for MediatR
        }
    }
}
