using Application.DTOs;
using Application.Exeption;
using Infrastructure.Auth.Interfaces;
using Infrastructure.Data;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, AuthenticatedUserDto>
    {
        private readonly AppDbContext _context;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IConfiguration _configuration;

        public AuthenticateUserCommandHandler(AppDbContext context, IJwtTokenGenerator tokenGenerator, IConfiguration configuration)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _configuration = configuration;
        }

        public async Task<AuthenticatedUserDto> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var cpfOnlyDigits = string.Empty;
            if (!string.IsNullOrWhiteSpace(request.Cpf))
                cpfOnlyDigits = new string(request.Cpf.Where(char.IsDigit).ToArray());

            var account = await _context.CheckingAccounts.FirstOrDefaultAsync(u => (u.Cpf == cpfOnlyDigits || u.AccountNumber == request.AccountNumber), cancellationToken);

            // CPF ou Número da conta não encontrados
            if (account == null)
                throw new ServiceException(ServiceError.Unauthorized);

            // Senha incorreta
            if (!EncryptionService.VerifyPassword(request.Password, account.Salt, account.Password))
                throw new ServiceException(ServiceError.Unauthorized);

            var token = _tokenGenerator.GenerateToken(account);

            return new AuthenticatedUserDto(token);
        }
    }
}
