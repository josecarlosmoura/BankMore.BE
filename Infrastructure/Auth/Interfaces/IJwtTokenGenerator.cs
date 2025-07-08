using Domain.Entities;

namespace Infrastructure.Auth.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ContaCorrente user);
    }
}
