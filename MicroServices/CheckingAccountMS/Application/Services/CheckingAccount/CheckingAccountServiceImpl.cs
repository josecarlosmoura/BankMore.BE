using BuildingBlocks.Exeption;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services.CheckingAccount
{
    public class CheckingAccountServiceImpl : ICheckingAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckingAccountServiceImpl(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetAccountId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new ServiceException(ServiceError.InvalidAccount);

            return userId;
        }
    }
}
