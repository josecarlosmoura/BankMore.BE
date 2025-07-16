namespace BuildingBlocks.Exeption
{
    public class ApiError
    {
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public ApiError(IServiceError error)
        {
            Message = error.Message;
            Type = error.Type;
        }
    }
}
