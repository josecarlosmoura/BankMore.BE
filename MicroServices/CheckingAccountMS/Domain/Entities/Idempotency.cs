namespace CheckingAccountMS.Domain.Entities
{
    public class Idempotency
    {
        public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString();
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public string Result { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}