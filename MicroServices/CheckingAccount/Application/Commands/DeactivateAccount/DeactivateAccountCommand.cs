using MediatR;

namespace Application.Commands.DeactivateAccount
{
    public class DeactivateAccountCommand : IRequest<Unit>
    {
        public string Password { get; set; } = string.Empty;
    }
}
