namespace App.Infrastructure.Services.Caching;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly string _instancePrefix;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(
        IConnectionMultiplexer multiplexer,
        IOptions<RedisSettings> options)
    {
        _db = multiplexer.GetDatabase();
        _instancePrefix = options.Value.InstanceName ?? string.Empty;
    }

    private string Build(string key) => $"{_instancePrefix}{key}";

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var value = await _db.StringGetAsync(Build(key));
        if (value.IsNullOrEmpty) return default;

        if (typeof(T) == typeof(string))
            return (T)(object)value.ToString();

        return JsonSerializer.Deserialize<T>(value!, JsonOptions);
    }

    public async Task SetAsync<T>(
        string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var payload = value is string s
            ? s
            : JsonSerializer.Serialize(value, JsonOptions);

        if (expiry.HasValue)
            await _db.StringSetAsync(Build(key), payload, (Expiration)expiry.Value);
        else
            await _db.StringSetAsync(Build(key), payload);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return _db.KeyDeleteAsync(Build(key));
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await _db.KeyExistsAsync(Build(key));
    }
}
