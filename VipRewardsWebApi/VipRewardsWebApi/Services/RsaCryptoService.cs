using System.Security.Cryptography;

namespace VipRewardsWebApi.Services;

public sealed class RsaCryptoService
{
    public string ExportPublicKeyPem(RSA rsa)
        => rsa.ExportSubjectPublicKeyInfoPem();

    public byte[] DecryptOaepSha256(RSA rsaPrivateKey, byte[] cipherBytes)
        => rsaPrivateKey.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);

    public static byte[] FromBase64(string b64, string fieldName)
    {
        try { return Convert.FromBase64String(b64); }
        catch { throw new CryptographicException($"{fieldName} is not valid Base64."); }
    }
}

