using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VipRewardsWebApi.Models;

public sealed class VipInfoQueryToken
{
    [JsonPropertyName("tk")]
    [Required]
    public string Tk { get; set; } = default!;
}