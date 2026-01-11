using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VipRewardsWebApi.Models;
public sealed class GetOneTimePublicKeyRequest
{
    [JsonPropertyName("requestId")]
    [Required]
    public string RequestId { get; set; } = default!;
}

