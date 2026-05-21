namespace App.Infrastructure.Options;

public class RedisSettings
{
    public const string SectionName = "RedisSettings";
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = string.Empty;
}
