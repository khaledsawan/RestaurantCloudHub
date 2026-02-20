namespace RestaurantSystem.Infrastructure.Options;

public class AuditOptions
{
    public string[] Tables { get; set; } = new[]
    {
        "users",
        "orders",
        "reservations",
        "refresh_tokens",
        "customers"
    };

    public string[] ExcludeColumns { get; set; } = new[]
    {
        "PasswordHash",
        "PasswordResetTokenHash",
        "RefreshToken",
        "Token",
        "Secret",
        "NewValues",
        "OldValues"
    };
}
