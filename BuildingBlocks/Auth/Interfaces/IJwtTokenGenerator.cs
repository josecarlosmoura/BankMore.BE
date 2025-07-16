using CheckingAccountMS.Domain.Entities;

namespace BuildingBlocks.Auth.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(CheckingAccount user);
    }
}
