using VipRewardsWebApi.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace VipRewardsWebApi.Services;

public sealed class HmacTokenService : IHmacTokenService
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;

    public HmacTokenService(IMemoryCache cache, IConfiguration config)
    {
        _cache = cache;
        _config = config;
    }

    public string IssueToken(string plainText)
    {
        var keySize = _config.GetValue("HmacToken:KeyBytes", 32);
        if (keySize < 16) keySize = 16;

        var key = RandomNumberGenerator.GetBytes(keySize);
        var hash = ComputeHmacSha256Hex(plainText, key);

        var record = new CachedHmacRecord
        {
            PlainText = plainText,
            KeyBase64 = Convert.ToBase64String(key),
            HashHex = hash,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var cacheMinutes = _config.GetValue("HmacToken:CacheMinutes", 10);
        if (cacheMinutes <= 0) cacheMinutes = 10;

        _cache.Set(hash, record, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheMinutes)
        });

        return hash;
    }

    public string RedeemToken(string token)
    {
        if (!_cache.TryGetValue(token, out CachedHmacRecord? record) || record is null)
            throw new KeyNotFoundException("Token not found or expired.");

        _cache.Remove(token); // 防重放

        return record.PlainText;
    }

    private static string ComputeHmacSha256Hex(string plainText, byte[] key)
    {
        var data = Encoding.UTF8.GetBytes(plainText);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
