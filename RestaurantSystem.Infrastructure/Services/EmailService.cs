using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Infrastructure.Services;

/// <summary>
/// Email service implementation
/// TODO: Integrate with actual email provider (SendGrid, AWS SES, etc.)
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;
    private readonly EmailSettings _settings;

    public EmailService(
        ILogger<EmailService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _settings = _configuration.GetSection("EmailSettings").Get<EmailSettings>()
            ?? throw new InvalidOperationException("EmailSettings configuration is missing");
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(new MailAddress(to));

            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass),
                EnableSsl = true
            };

            await client.SendMailAsync(message);

            _logger.LogInformation("Email sent to {To}, Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true)
    {
        foreach (var recipient in to)
        {
            await SendEmailAsync(recipient, subject, body, isHtml);
        }
    }

    public async Task SendTemplatedEmailAsync(string to, string templateName, object model)
    {
        // TODO: Implement template rendering
        var subject = $"Email from Restaurant System - {templateName}";
        var body = $"Template: {templateName}";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendOrderConfirmationAsync(string to, int orderId, string orderNumber)
    {
        var subject = $"Order Confirmation - {orderNumber}";
        var body = $@"
            <h1>Thank you for your order!</h1>
            <p>Your order #{orderNumber} has been confirmed.</p>
            <p>Order ID: {orderId}</p>
            <p>We'll notify you when your order is ready.</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendOrderStatusUpdateAsync(string to, int orderId, string orderNumber, string newStatus)
    {
        var subject = $"Order Update - {orderNumber}";
        var body = $@"
            <h1>Order Status Update</h1>
            <p>Your order #{orderNumber} status has been updated to: <strong>{newStatus}</strong></p>
            <p>Order ID: {orderId}</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendReservationConfirmationAsync(string to, string confirmationCode, DateTime reservationDate)
    {
        var subject = "Reservation Confirmed";
        var body = $@"
            <h1>Reservation Confirmed!</h1>
            <p>Your reservation has been confirmed.</p>
            <p>Confirmation Code: <strong>{confirmationCode}</strong></p>
            <p>Date: {reservationDate:MMMM dd, yyyy}</p>
            <p>Time: {reservationDate:hh:mm tt}</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken)
    {
        var subject = "Password Reset Request";
        var body = $@"
            <h1>Password Reset</h1>
            <p>You requested a password reset.</p>
            <p>Use this token to reset your password: <strong>{resetToken}</strong></p>
            <p>This token expires in 24 hours.</p>
            <p>If you didn't request this, please ignore this email.</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string to, string firstName)
    {
        var subject = "Welcome to Our Restaurant!";
        var body = $@"
            <h1>Welcome, {firstName}!</h1>
            <p>Thank you for joining us!</p>
            <p>We're excited to serve you delicious food.</p>
            <p>Start exploring our menu and place your first order today!</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendEmailConfirmationAsync(string to, string code, string firstName)
    {
        var subject = "Confirm your email";
        var body = $@"
            <h1>Hi {firstName},</h1>
            <p>Thanks for registering! Use the code below to confirm your email:</p>
            <p><strong>{code}</strong></p>
            <p>This code expires in 24 hours.</p>
            <p>If you did not create this account, you can ignore this email.</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetCodeAsync(string to, string code, string firstName)
    {
        var subject = "Reset your password";
        var body = $@"
            <h1>Hi {firstName},</h1>
            <p>Use the code below to reset your password:</p>
            <p><strong>{code}</strong></p>
            <p>This code expires in 24 hours.</p>
            <p>If you did not request a password reset, you can ignore this email.</p>
        ";

        await SendEmailAsync(to, subject, body);
    }
}

internal sealed class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPass { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
}
