using System.Net;

namespace Application.Exeption
{
    public class ApiError
    {
        public HttpStatusCode Status { get; set; }
        public string Type { get; set; } = string.Empty;

        public ApiError(IServiceError error)
        {
            Status = error.Status;
            Type = error.Type;
        }
    }
}
