using CheckingAccountMS.Domain.Enuns;
using MediatR;

namespace CheckingAccountMS.Application.Commands.CreateTransaction
{
    public class CreateTransactionCommand : IRequest<string>
    {
        public Guid IdempotencyKey { get; set; }
        public long? AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
