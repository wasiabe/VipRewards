namespace VipRewardsWebApi.Models;

public sealed class CachedHmacRecord
{
    public string PlainText { get; init; } = string.Empty;
    public string KeyBase64 { get; init; } = string.Empty;
    public string HashHex { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}
