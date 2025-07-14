using BuildingBlocks.Exeption;
using BuildingBlocks.Security;
using CheckingAccountMS.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CheckingAccountMS.Application.Commands.DeactivateAccount
{
    public class DeactivateAccountCommandHandler : IRequestHandler<DeactivateAccountCommand, Unit>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeactivateAccountCommandHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
        {
            var accountId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountId))
                throw new UnauthorizedAccessException("Invalid or expired token.");

            var account = await _context.CheckingAccounts
            .FirstOrDefaultAsync(a => a.CheckingAccountId == accountId, cancellationToken);

            if (account == null)
                throw new ServiceException(ServiceError.InvalidAccount);

            var isPasswordValid = EncryptionService.VerifyPassword(request.Password, account.Salt, account.Password);

            if (!isPasswordValid)
                throw new ServiceException(ServiceError.IncorrectPassword);            

            account.IsActive = false;

            _context.CheckingAccounts.Update(account);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
