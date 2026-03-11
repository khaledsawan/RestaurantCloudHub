namespace Infrastructure.RateLimiting;

public static class RateLimiterPolicies
{
    public const string Login = "login-limit";
    public const string Register = "register-limit";
    public const string SendOtp = "otp-limit";
    public const string Global = "global-limit";
}