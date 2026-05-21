namespace App.Infrastructure.Services.Otp;

public sealed class OtpService : IOtpService
{
    private readonly ICacheService     _cache;
    private readonly IDateTimeProvider _clock;
    private readonly OtpSettings       _settings;

    public OtpService(
        ICacheService cache,
        IDateTimeProvider clock,
        IOptions<OtpSettings> settings)
    {
        _cache = cache;
        _clock = clock;
        _settings = settings.Value;
    }

    public async Task<string> GenerateAndStoreAsync(
        string phoneNumber, CancellationToken ct = default)
    {
        var normalized = Normalize(phoneNumber);

        if (await _cache.ExistsAsync(CooldownKey(normalized), ct))
            throw new BusinessRuleException(
                "Please wait before requesting another code.",
                ErrorCodes.OtpResendCooldown);

        var code = GenerateNumericCode(_settings.Length);

        var record = new OtpRecord(
            Code: code,
            CreatedAtUtc: _clock.UtcNow,
            ExpiresAtUtc: _clock.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            Attempts: 0);

        var ttl = TimeSpan.FromMinutes(_settings.ExpirationMinutes);
        await _cache.SetAsync(OtpKey(normalized), record, ttl, ct);

        await _cache.SetAsync(
            CooldownKey(normalized),
            value: "1",
            expiry: TimeSpan.FromSeconds(_settings.ResendCooldownSeconds),
            ct: ct);

        return code;
    }

    public async Task<bool> ValidateAsync(
        string phoneNumber, string code, CancellationToken ct = default)
    {
        var normalized = Normalize(phoneNumber);
        var key = OtpKey(normalized);

        var record = await _cache.GetAsync<OtpRecord>(key, ct);
        if (record is null)
            throw new BusinessRuleException(
                "OTP has expired or was not requested.",
                ErrorCodes.OtpExpired);

        if (record.Attempts >= _settings.MaxAttempts)
        {
            await _cache.RemoveAsync(key, ct);
            throw new BusinessRuleException(
                "Maximum OTP attempts exceeded. Please request a new code.",
                ErrorCodes.OtpMaxAttempts);
        }

        var isMatch = FixedTimeEquals(record.Code, code?.Trim() ?? string.Empty);

        if (!isMatch)
        {
            var updated = record with { Attempts = record.Attempts + 1 };
            var remainingTtl = record.ExpiresAtUtc - _clock.UtcNow;
            if (remainingTtl > TimeSpan.Zero)
                await _cache.SetAsync(key, updated, remainingTtl, ct);
            return false;
        }

        await _cache.RemoveAsync(key, ct);
        return true;
    }

    public Task InvalidateAsync(string phoneNumber, CancellationToken ct = default)
        => _cache.RemoveAsync(OtpKey(Normalize(phoneNumber)), ct);

    public async Task<bool> CanResendAsync(string phoneNumber, CancellationToken ct = default)
        => !await _cache.ExistsAsync(CooldownKey(Normalize(phoneNumber)), ct);

    private string OtpKey(string phone) => $"{_settings.KeyPrefix}{phone}";
    private string CooldownKey(string phone) => $"{_settings.KeyPrefix}{phone}:cooldown";

    private static string Normalize(string phoneNumber)
        => string.IsNullOrWhiteSpace(phoneNumber)
            ? throw new ArgumentException("Phone number is required.", nameof(phoneNumber))
            : phoneNumber.Trim();

    private static string GenerateNumericCode(int length)
    {
        Span<byte> buffer = stackalloc byte[4];
        var sb = new StringBuilder(length);

        while (sb.Length < length)
        {
            RandomNumberGenerator.Fill(buffer);
            var digit = BitConverter.ToUInt32(buffer) % 10;
            sb.Append(digit);
        }
        return sb.ToString();
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        var bytesA = Encoding.UTF8.GetBytes(a);
        var bytesB = Encoding.UTF8.GetBytes(b);
        return CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
    }
}
