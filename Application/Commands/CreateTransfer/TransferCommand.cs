using FluentResults;
using MediatR;

namespace TransferMS.Application.Commands.CreateTransfer
{
    public class TransferCommand : IRequest<Result>
    {
        public Guid IdempotencyKey { get; init; } = Guid.NewGuid();
        public long DestinationAccountNumber { get; init; } = default!;
        public decimal Amount { get; init; }
    }
}
