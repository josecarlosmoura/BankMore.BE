using BuildingBlocks.Auth.Interfaces;
using BuildingBlocks.Settings;
using CheckingAccountMS.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuildingBlocks.Auth
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;

        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public string GenerateToken(CheckingAccount contaCorrente)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, contaCorrente.CheckingAccountId.ToString()),
            new Claim("account_id", contaCorrente.AccountNumber.ToString()),
            new Claim(ClaimTypes.Name, contaCorrente.FullName)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
