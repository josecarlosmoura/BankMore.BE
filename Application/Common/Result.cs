namespace TransferMS.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? Message { get; private set; }
        public string? FailureType { get; private set; }

        public static Result Success() => new Result { IsSuccess = true };
        public static Result Failure(string message, string failureType) => new Result
        {
            IsSuccess = false,
            Message = message,
            FailureType = failureType
        };
    }
}
