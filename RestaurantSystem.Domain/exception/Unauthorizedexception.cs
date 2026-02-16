namespace RestaurantSystem.Domain.Exceptions;

/// <summary>
/// Exception thrown when user is not authenticated
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException()
        : base("User is not authenticated")
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
