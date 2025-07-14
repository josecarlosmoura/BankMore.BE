using BuildingBlocks.Exeption;
using Microsoft.AspNetCore.Http;

namespace Application.Services.CheckingAccount
{
    public class CheckingAccountServiceImpl : ICheckingAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckingAccountServiceImpl(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long GetAccountId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var accountIdClaim = user?.FindFirst("account_id")?.Value;

            if (accountIdClaim == null)
                throw new ServiceException(ServiceError.InvalidAccount);

            return long.Parse(accountIdClaim);
        }
    }
}
