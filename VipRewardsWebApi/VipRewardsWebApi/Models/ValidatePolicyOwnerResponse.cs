using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VipRewardsWebApi.Models;

public sealed class ValidatePolicyOwnerResponse
{
    [JsonPropertyName("tokens")]
    [Required]
    public List<VipInfoQueryToken> Tokens { get; set; } = default!;
}


