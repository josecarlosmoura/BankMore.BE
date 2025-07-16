using Application.DTOs;
using BuildingBlocks.Auth;
using BuildingBlocks.Exeption;
using BuildingBlocks.Security;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using CheckingAccountMS.Infrastructure.Utils;
using MediatR;

namespace CheckingAccountMS.Application.Commands.CreateAccount
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
    {
        private readonly ICheckingAccountRepository _checkingAccountRepository;

        public CreateAccountCommandHandler(ICheckingAccountRepository checkingAccountRepository)
        {
            _checkingAccountRepository = checkingAccountRepository;
        }

        public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // Verifica se o CPF é válido
            if (!CpfValidator.EhCpfValido(request.Cpf))
            {
                throw new ServiceException(ServiceError.InvalidDocument);
            }

            var cpfOnlyDigits = new string(request.Cpf.Where(char.IsDigit).ToArray());

            // Verifica se já existe uma conta com o mesmo CPF
            var existingAccount = await _checkingAccountRepository.FirstOrDefaultAsync(c => c.Cpf == cpfOnlyDigits);
            if (existingAccount != null)
            {
                throw new ServiceException(ServiceError.UserAlreadyExists);
            }

            var contaCorrenteNumber = long.Parse(ContaCorrenteGenerator.GerarNumeroContaComDV().Replace("-", ""));

            // Gera senha criptografada
            var salt = PasswordHasher.GenerateSalt();
            var hash = PasswordHasher.HashPassword(request.Password, salt);
            var encryptedPassword = EncryptionService.Encrypt(request.Password + salt);

            // Cria a nova conta
            var account = new CheckingAccount
            {
                AccountNumber = contaCorrenteNumber,
                Cpf = cpfOnlyDigits,
                FullName = request.Name,
                IsActive = true,
                Salt = salt,
                Password = encryptedPassword
            };

            // Salva a conta no banco de dados
            await _checkingAccountRepository.AddAsync(account);
            await _checkingAccountRepository.SaveChangesAsync(cancellationToken);

            return new AccountDto(account);
        }
    }
}
