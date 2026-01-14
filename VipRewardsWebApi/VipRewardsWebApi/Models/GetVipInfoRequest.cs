using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VipRewardsWebApi.Models
{
    public sealed class GetVipInfoRequest
    {
        /// <summary>
        /// 一次性Token
        /// </summary>
        [JsonPropertyName("tk")]
        [Required]
        public string Tk { get; set; } = string.Empty;
    }
}

