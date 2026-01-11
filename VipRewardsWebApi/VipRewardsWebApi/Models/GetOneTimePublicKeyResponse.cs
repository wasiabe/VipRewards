using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VipRewardsWebApi.Models;

public sealed class GetOneTimePublicKeyResponse
{
    [JsonPropertyName("publicKey")]
    [Required]
    // PEM (SubjectPublicKeyInfo) 格式
    public string PublicKey { get; set; } = default!;
}
