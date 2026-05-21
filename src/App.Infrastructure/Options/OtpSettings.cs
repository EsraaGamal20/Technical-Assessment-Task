namespace App.Infrastructure.Options;

public class OtpSettings
{
    public const string SectionName = "OtpSettings";
    public int Length { get; set; }
    public int ExpirationMinutes { get; set; }
    public int MaxAttempts { get; set; }
    public int ResendCooldownSeconds { get; set; }
    public string KeyPrefix { get; set; } = string.Empty;
}
