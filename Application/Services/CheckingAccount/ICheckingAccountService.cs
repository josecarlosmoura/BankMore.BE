namespace TransferMS.Application.Services.CheckingAccount
{
    public interface ICheckingAccountService
    {
        string AccountId { get; }
        string JwtToken { get; }
    }
}
