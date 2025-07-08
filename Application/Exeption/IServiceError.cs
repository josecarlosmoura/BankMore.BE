using System.Net;

namespace Application.Exeption
{
    public interface IServiceError
    {
        HttpStatusCode Status { get; }
        string Type { get; }
    }
}
