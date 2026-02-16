namespace RestaurantSystem.Domain.Exceptions;

/// <summary>
/// Exception thrown when user lacks permission for an action
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException()
        : base("Access forbidden")
    {
    }

    public ForbiddenException(string message)
        : base(message)
    {
    }

    public ForbiddenException(string resource, string action)
        : base($"Access forbidden: Cannot {action} {resource}")
    {
    }
}