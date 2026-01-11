using System.Security.Cryptography;
using System.Text;

namespace VipRewardsWebApi.Services;

public sealed class AesGcmCryptoService
{
    // key: 16/24/32 bytes (AES-128/192/256)
    // iv: 建議 12 bytes
    // tag: 常見 16 bytes
    public string DecryptToUtf8(byte[] key, byte[] iv, byte[] cipherText, byte[] tag)
    {
        var plainBytes = new byte[cipherText.Length];

        try
        {
            using var aes = new AesGcm(key, tagSizeInBytes: tag.Length);
            aes.Decrypt(iv, cipherText, tag, plainBytes);
        }
        catch (CryptographicException)
        {
            // 常見原因：Tag 不對（資料被竄改 / key 錯 / iv 錯 / cipherText 損毀）
            throw;
        }

        return Encoding.UTF8.GetString(plainBytes);
    }
}

