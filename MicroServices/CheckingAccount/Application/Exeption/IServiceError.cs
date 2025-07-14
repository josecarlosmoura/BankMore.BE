namespace Application.Exeption
{
    public interface IServiceError
    {
        string Message { get; }
        string Type { get; }
    }
}
