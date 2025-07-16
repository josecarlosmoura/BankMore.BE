namespace BuildingBlocks.Exeption
{
    public interface IServiceError
    {
        string Message { get; }
        string Type { get; }
    }
}
