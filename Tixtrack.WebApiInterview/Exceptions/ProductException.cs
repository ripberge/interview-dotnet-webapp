namespace TixTrack.WebApiInterview.Exceptions;

public abstract class ProductException : Exception
{
    protected ProductException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class InvalidProductQuantityException : ProductException
{
    public InvalidProductQuantityException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class InvalidProductIdException : ProductException
{
    public InvalidProductIdException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class UnavailableProductQuantityException : ProductException
{
    public UnavailableProductQuantityException(
        string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}