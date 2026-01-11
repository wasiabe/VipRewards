using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VipRewardsWebApi.Models;

public sealed class GetVipInfoResponse
{
    [JsonPropertyName("id")]
    [Required]
    public string Id { get; set; } = default!;

    [JsonPropertyName("name")]
    [Required]
    public string Name { get; set; } = default!;

    [JsonPropertyName("vipLevel")]
    [Required]
    public string VipLevel { get; set; } = default!;

    [JsonPropertyName("rewardBalance")]
    [Required]
    public int RewardBalance { get; set; } = default!;
}
