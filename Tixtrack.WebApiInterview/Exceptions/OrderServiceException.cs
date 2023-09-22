namespace TixTrack.WebApiInterview.Exceptions;

public abstract class OrderServiceException : Exception
{
    protected OrderServiceException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class OrderIsNotActiveException : OrderServiceException
{
    public OrderIsNotActiveException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class OrderNotFoundException : OrderServiceException
{
    public OrderNotFoundException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class InvalidProductQuantityException : OrderServiceException
{
    public InvalidProductQuantityException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class InvalidProductIdException : OrderServiceException
{
    public InvalidProductIdException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}