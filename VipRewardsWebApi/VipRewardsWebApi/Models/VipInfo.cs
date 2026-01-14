using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VipRewardsWebApi.Models;

public sealed class VipInfo
{
    [JsonPropertyName("vipinfoId")]
    [Required]
    public string VipInfoId { get; set; } = default!;

    [JsonPropertyName("name")]
    [Required]
    public string Name { get; set; } = default!;

    [JsonPropertyName("vipLevel")]
    [Required]
    public string VipLevel { get; set; } = default!;

    [JsonPropertyName("validFrom")]
    [Required]
    public string ValidFrom { get; set; } = default!;

    [JsonPropertyName("validTill")]
    [Required]
    public string ValidTill { get; set; } = default!;

    [JsonPropertyName("rewardBalance")]
    [Required]
    public int RewardBalance { get; set; } = default!;
}
