namespace TixTrack.WebApiInterview.Exceptions;

public abstract class OrderServiceException : Exception
{
}

public class OrderIsNotActiveException : OrderServiceException
{
}

public class OrderNotFoundException : OrderServiceException
{
}