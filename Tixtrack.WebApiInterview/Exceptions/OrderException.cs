namespace TixTrack.WebApiInterview.Exceptions;

public abstract class OrderException : Exception
{
    protected OrderException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class OrderIsNotActiveException : OrderException
{
    public OrderIsNotActiveException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class OrderNotFoundException : OrderException
{
    public OrderNotFoundException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}