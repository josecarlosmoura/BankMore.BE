using TransferMS.Application.Models;

namespace Application.DTOs
{
    public class ResultToIdempotencyDto
    {
        public Guid IdempotencyKeyFromDebit { get; set; }
        public OperationResult DebitResult { get; set; }
        public Guid IdempotencyKeyFromCredit { get; set; }
        public OperationResult CreditResult { get; set; }
        public Guid? IdempotencyKeyFromTransferRefund { get; set; }
        public OperationResult? TransferRefundResult { get; set; }
    }
}
