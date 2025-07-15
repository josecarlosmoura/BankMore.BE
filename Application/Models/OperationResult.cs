namespace TransferMS.Application.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? AccountId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
