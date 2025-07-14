using Application.DTOs;
using BuildingBlocks.Auth;
using BuildingBlocks.Exeption;
using BuildingBlocks.Security;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Infrastructure.Data;
using CheckingAccountMS.Infrastructure.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CheckingAccountMS.Application.Commands.CreateAccount
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
    {
        private readonly AppDbContext _context;

        public CreateAccountCommandHandler(AppDbContext context)
        {
            _context = context;
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
            var existingAccount = await _context.CheckingAccounts
                .FirstOrDefaultAsync(c => c.Cpf == cpfOnlyDigits, cancellationToken);
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
            _context.CheckingAccounts.Add(account);
            await _context.SaveChangesAsync(cancellationToken);

            return new AccountDto(account);
        }
    }
}
