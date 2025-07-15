namespace BuildingBlocks.Exeption
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
        public static readonly ServiceError Unauthorized = new ("Cpf/Número da conta ou senha incorretos", "USER_UNAUTHORIZED");
        public static readonly ServiceError InvalidAccount = new("Conta inválida", "INVALID_ACCOUNT");
        public static readonly ServiceError IncorrectPassword = new ("Senha incorreta", "INCORRECT_PASSWORD");
        public static readonly ServiceError InactiveAccount = new ("Conta inativa", "INACTIVE_ACCOUNT");
        public static readonly ServiceError InvalidValue = new("Valor inválido", "INVALID_VALUE");
        public static readonly ServiceError InvalidType = new("Tipo inválido", "INVALID_TYPE");
        public static readonly ServiceError InsufficientBalance = new("Saldo insuficiente", "INSUFFICIENT_BALANCE");
        public static readonly ServiceError ClaimNotFound = new("Claim 'account_id' não encontrada no token", "CLAIM_NOT_FOUND");
        public static readonly ServiceError DebitFailed = new("Falha ao debitar", "DEBIT_FAILED");
        public static readonly ServiceError CreditFailed = new("Falha ao creditar. Estorno realizado", "CREDIT_FAILED");
    }
}
