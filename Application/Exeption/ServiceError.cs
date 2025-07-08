using System.Net;

namespace Application.Exeption
{
    public class ServiceError : IServiceError
    {
        public HttpStatusCode Status { get; }
        public string Type { get; }

        private ServiceError(HttpStatusCode status, string type)
        {
            Status = status;
            Type = type;
        }

        // Adicionar outros erros aqui no futuro
        public static readonly ServiceError InvalidDocument = new (HttpStatusCode.BadRequest, "INVALID_DOCUMENT");
        public static readonly ServiceError UserAlreadyExists = new (HttpStatusCode.Conflict, "USER_ALREADY_EXISTS");
        public static readonly ServiceError Unauthorized = new (HttpStatusCode.Unauthorized, "USER_UNAUTHORIZED");
    }
}
