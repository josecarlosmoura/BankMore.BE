using System.Security.Cryptography;

namespace BuildingBlocks.Auth
{
    public class PasswordHasher
    {
        // Gera um salt seguro
        public static string GenerateSalt(int size = 16)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToBase64String(saltBytes);
        }

        // Gera o hash da senha com salt usando PBKDF2
        public static string HashPassword(string password, string salt, int iterations = 100_000)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(pbkdf2.GetBytes(32)); // 256 bits
        }
    }
}
