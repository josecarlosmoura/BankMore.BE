using Application.DTOs;
using Application.Exeption;
using Domain.Entities;
using Infrastructure.Auth;
using Infrastructure.Data;
using Infrastructure.Security;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class ContaCorrenteService
    {
        private readonly AppDbContext _context;

        public ContaCorrenteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ContaCorrente>> GetAllAsync() => await _context.ContaCorrente.ToListAsync();

        public async Task<ContaCorrente?> GetByIdAsync(int id) => await _context.ContaCorrente.FindAsync(id);

        public async Task<ContaCorrente> CriarAsync(CreateContaCorrenteDto dto)
        {
            if (!CpfValidator.EhCpfValido(dto.Cpf))
                throw new ServiceException(ServiceError.InvalidDocument);

            var salt = PasswordHasher.GenerateSalt();
            var hash = PasswordHasher.HashPassword(dto.Senha, salt);
            var senhaCriptografada = EncryptionService.Encrypt(dto.Senha + salt); // Concatena senha + salt

            // Todo - Verificar se já existe uma conta com o mesmo CPF
            var contaCorrenteNumber = long.Parse(ContaCorrenteGenerator.GerarNumeroContaComDV().Replace("-", ""));

            var conta = new ContaCorrente
            {
                Numero = contaCorrenteNumber,
                Cpf = long.Parse(new string(dto.Cpf.Where(char.IsDigit).ToArray())),
                Nome = dto.Nome,
                Ativo = true,
                Salt = salt,
                Senha = senhaCriptografada
            };

            _context.ContaCorrente.Add(conta);
            await _context.SaveChangesAsync();

            return conta;
        }
    }
}
