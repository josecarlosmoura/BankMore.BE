namespace Application.Exeption
{
    public class ServiceError : IServiceError
    {
        public string Message { get; }
        public string Type { get; }

        private ServiceError(string message, string type)
        {
            Message = message;
            Type = type;
        }

        // Adicionar outros erros aqui no futuro
        public static readonly ServiceError InvalidDocument = new ("Cpf inválido", "INVALID_DOCUMENT");
        public static readonly ServiceError UserAlreadyExists = new ("Usuário já existe", "USER_ALREADY_EXISTS");
        public static readonly ServiceError Unauthorized = new ("Usuário não autorizado para este aperação", "USER_UNAUTHORIZED");
    }
}
