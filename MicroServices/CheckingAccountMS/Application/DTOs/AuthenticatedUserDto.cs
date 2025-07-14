namespace CheckingAccountMS.Application.DTOs
{
    public class AuthenticatedUserDto
    {
        public string Token { get; set; }

        public AuthenticatedUserDto(string token)
        {
            Token = token;
        }
    }
}
