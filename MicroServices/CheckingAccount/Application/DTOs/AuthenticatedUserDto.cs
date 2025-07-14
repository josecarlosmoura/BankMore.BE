namespace Application.DTOs
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
