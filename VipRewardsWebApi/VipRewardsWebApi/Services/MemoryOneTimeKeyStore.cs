using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace VipRewardsWebApi.Services;

public sealed class MemoryOneTimeKeyStore : IOneTimeKeyStore
{
    private readonly IMemoryCache _cache;

    public MemoryOneTimeKeyStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Put(string requestId, RSA rsaPrivateKey, TimeSpan ttl)
    {
        // 可改成：如果已存在就拒絕 (防覆蓋)
        _cache.Remove(requestId);

        _cache.Set(
            requestId,
            rsaPrivateKey,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });
    }

    public RSA? Take(string requestId)
    {
        if (_cache.TryGetValue<RSA>(requestId, out var rsa))
        {
            _cache.Remove(requestId); // 一次性：取出即刪
            return rsa;
        }
        return null;
    }
}
