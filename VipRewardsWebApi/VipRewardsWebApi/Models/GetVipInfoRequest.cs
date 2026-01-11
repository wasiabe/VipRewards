using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VipRewardsWebApi.Models
{
    /// <summary>
    /// API 請求用的加密封包實體，內容為外層封裝的加密資料。
    /// JSON 範例:
    /// {
    ///   "requestId": "uuid",
    ///   "encryptedKey": "Base64",
    ///   "iv": "Base64",
    ///   "cipherText": "Base64",
    ///   "tag": "Base64"
    /// }
    /// </summary>
    public sealed class GetVipInfoRequest
    {
        /// <summary>
        /// 請求識別碼 (UUID)，用於追蹤與去重。
        /// </summary>
        [JsonPropertyName("requestId")]
        [Required]
        public required string RequestId { get; init; }

        /// <summary>
        /// 使用 RSA-OAEP 加密後的 AES Key，Base64 編碼。
        /// </summary>
        [JsonPropertyName("encryptedKey")]
        [Required]
        public required string EncryptedKey { get; init; }

        /// <summary>
        /// AES-GCM 初始化向量 (IV)，Base64 編碼。
        /// </summary>
        [JsonPropertyName("iv")]
        [Required]
        public required string Iv { get; init; }

        /// <summary>
        /// 使用 AES-GCM 加密後的資料，Base64 編碼。
        /// </summary>
        [JsonPropertyName("cipherText")]
        [Required]
        public required string CipherText { get; init; }

        /// <summary>
        /// AES-GCM 驗證標籤 (tag)，Base64 編碼。
        /// </summary>
        [JsonPropertyName("tag")]
        [Required]
        public required string Tag { get; init; }
    }
}

