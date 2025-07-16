using BuildingBlocks.Auth.Interfaces;
using BuildingBlocks.Exeption;
using BuildingBlocks.Security;
using CheckingAccountMS.Application.DTOs;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using MediatR;

namespace CheckingAccountMS.Application.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, AuthenticatedUserDto>
    {
        private readonly ICheckingAccountRepository _checkingAccountRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthenticateUserCommandHandler(ICheckingAccountRepository checkingAccountRepository, IJwtTokenGenerator tokenGenerator)
        {
            _checkingAccountRepository = checkingAccountRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthenticatedUserDto> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var cpfOnlyDigits = string.Empty;
            if (!string.IsNullOrWhiteSpace(request.Cpf))
                cpfOnlyDigits = new string(request.Cpf.Where(char.IsDigit).ToArray());

            var account = await _checkingAccountRepository.FirstOrDefaultAsync(u => u.Cpf == cpfOnlyDigits || u.AccountNumber == request.AccountNumber);

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
