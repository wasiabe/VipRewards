using System.Security.Cryptography;

namespace VipRewardsWebApi.Services;

public interface IOneTimeKeyStore
{
    // Put: 放入 requestId -> RSA private key (一次性)
    void Put(string requestId, RSA rsaPrivateKey, TimeSpan ttl);

    // Take: 取出並刪除（一次性）
    RSA? Take(string requestId);
}

