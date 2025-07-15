using FluentResults;
using MediatR;

namespace TransferMS.Application.Commands.CreateTransfer
{
    public class TransferCommand : IRequest<Result>
    {
        public string RequestId { get; init; } = default!;
        public string DestinationAccountNumber { get; init; } = default!;
        public decimal Amount { get; init; }
    }
}
