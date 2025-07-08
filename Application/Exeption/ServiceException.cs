namespace Application.Exeption
{
    public class ServiceException : Exception
    {
        public IServiceError Error { get; }

        public ServiceException(IServiceError error) : base(error.Type)
        {
            Error = error;
        }
    }
}
