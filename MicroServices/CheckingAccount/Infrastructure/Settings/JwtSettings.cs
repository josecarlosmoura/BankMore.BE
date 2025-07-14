namespace Infrastructure.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = "BankMoreIssuer";
        public string Audience { get; set; } = "BankMoreAudience";
        public uint ExpirationMinutes { get; set; } = 60;
    }
}
