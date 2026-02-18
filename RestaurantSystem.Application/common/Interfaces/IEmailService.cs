namespace RestaurantSystem.Application.Common.Interfaces;

/// <summary>
/// Interface for email operations
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send email to single recipient
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Send email to multiple recipients
    /// </summary>
    Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Send email using template
    /// </summary>
    Task SendTemplatedEmailAsync(string to, string templateName, object model);

    /// <summary>
    /// Send order confirmation email
    /// </summary>
    Task SendOrderConfirmationAsync(string to, int orderId, string orderNumber);

    /// <summary>
    /// Send order status update email
    /// </summary>
    Task SendOrderStatusUpdateAsync(string to, int orderId, string orderNumber, string newStatus);

    /// <summary>
    /// Send reservation confirmation email
    /// </summary>
    Task SendReservationConfirmationAsync(string to, string confirmationCode, DateTime reservationDate);

    /// <summary>
    /// Send password reset email
    /// </summary>
    Task SendPasswordResetEmailAsync(string to, string resetToken);

    /// <summary>
    /// Send welcome email for new customers
    /// </summary>
    Task SendWelcomeEmailAsync(string to, string firstName);

    /// <summary>
    /// Send email confirmation code
    /// </summary>
    Task SendEmailConfirmationAsync(string to, string code, string firstName);

    /// <summary>
    /// Send password reset code
    /// </summary>
    Task SendPasswordResetCodeAsync(string to, string code, string firstName);
}
