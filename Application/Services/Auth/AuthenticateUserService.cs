using Application.Exeption;
using Infrastructure.Auth;
using Infrastructure.Auth.Interfaces;
using Infrastructure.Data;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

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

        public async Task<string?> AuthenticateAsync(string cpf, long numero, string password)
        {
            var cpfOnlyDigits = new string(cpf.Where(char.IsDigit).ToArray());

            var account = await _db.ContaCorrente.FirstOrDefaultAsync(u => (u.Cpf == cpfOnlyDigits || u.Numero == numero));

            if(account == null)
                throw new ServiceException(ServiceError.Unauthorized); // CPF não encontrado

            if (!EncryptionService.VerifyPassword(password, account.Salt, account.Senha))
                throw new ServiceException(ServiceError.Unauthorized); // Senha incorreta

            return _tokenGenerator.GenerateToken(account);
        }
    }
}
