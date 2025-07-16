using BuildingBlocks.Exeption;
using BuildingBlocks.Security;
using CheckingAccountMS.Infrastructure.Data;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CheckingAccountMS.Application.Commands.DeactivateAccount
{
    public class DeactivateAccountCommandHandler : IRequestHandler<DeactivateAccountCommand, Unit>
    {
        private readonly ICheckingAccountRepository _checkingAccountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeactivateAccountCommandHandler(ICheckingAccountRepository checkingAccountRepository, IHttpContextAccessor httpContextAccessor)
        {
            _checkingAccountRepository = checkingAccountRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
        {
            var accountId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountId))
                throw new UnauthorizedAccessException("Invalid or expired token.");

            var account = await _checkingAccountRepository.FirstOrDefaultAsync(a => a.CheckingAccountId == accountId);

            if (account == null)
                throw new ServiceException(ServiceError.InvalidAccount);

            var isPasswordValid = EncryptionService.VerifyPassword(request.Password, account.Salt, account.Password);

            if (!isPasswordValid)
                throw new ServiceException(ServiceError.IncorrectPassword);            

            account.IsActive = false;

            _checkingAccountRepository.Update(account);
            await _checkingAccountRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
