using Application.DTOs;
using MediatR;

namespace CheckingAccountMS.Application.Commands.CreateAccount
{
    public class CreateAccountCommand : IRequest<AccountDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
    }
}
