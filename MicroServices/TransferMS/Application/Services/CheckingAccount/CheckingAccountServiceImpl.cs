using Microsoft.AspNetCore.Http;
using TransferMS.Application.Services.CheckingAccount;

namespace Application.Services.CheckingAccount
{
    public class CheckingAccountServiceImpl : ICheckingAccountService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CheckingAccountServiceImpl(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string AccountId => _contextAccessor.HttpContext?.User?.FindFirst("account_id")?.Value ?? string.Empty;
        public string JwtToken => _contextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString().Replace("Bearer ", "");
    }
}
