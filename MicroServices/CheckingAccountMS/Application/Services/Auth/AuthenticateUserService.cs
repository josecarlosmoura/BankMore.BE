using BuildingBlocks.Auth.Interfaces;
using BuildingBlocks.Exeption;
using BuildingBlocks.Security;
using CheckingAccountMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Auth
{
    public class AuthenticateUserService
    {
        private readonly AppDbContext _db;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthenticateUserService(AppDbContext db, IJwtTokenGenerator tokenGenerator)
        {
            _db = db;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string?> AuthenticateAsync(string cpf, long accountNumber, string password)
        {
            var cpfOnlyDigits = new string(cpf.Where(char.IsDigit).ToArray());

            var account = await _db.CheckingAccounts.FirstOrDefaultAsync(u => (u.Cpf == cpfOnlyDigits || u.AccountNumber == accountNumber));

            if(account == null)
                throw new ServiceException(ServiceError.Unauthorized); // CPF não encontrado

            if (!EncryptionService.VerifyPassword(password, account.Salt, account.Password))
                throw new ServiceException(ServiceError.Unauthorized); // Senha incorreta

            return _tokenGenerator.GenerateToken(account);
        }
    }
}
