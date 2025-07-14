using Application.DTOs;
using MediatR;

namespace Application.Commands.AuthenticateUser
{
    public class AuthenticateUserCommand : IRequest<AuthenticatedUserDto>
    {
        public string? Cpf { get; set; }
        public long? AccountNumber { get; set; }
        public string Password { get; set; }
    }
}
