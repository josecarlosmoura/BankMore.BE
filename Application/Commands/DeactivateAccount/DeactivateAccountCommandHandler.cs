using Application.Exeption;
using Infrastructure.Data;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Commands.DeactivateAccount
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

            var account = await _context.ContaCorrente
            .FirstOrDefaultAsync(a => a.IdContaCorrente == accountId, cancellationToken);

            if (account == null)
                throw new ServiceException(ServiceError.InvalidAccount);

            var isPasswordValid = EncryptionService.VerifyPassword(request.Password, account.Salt, account.Senha);

            if (!isPasswordValid)
                throw new ServiceException(ServiceError.IncorrectPassword);            

            account.Ativo = false;

            _context.ContaCorrente.Update(account);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
