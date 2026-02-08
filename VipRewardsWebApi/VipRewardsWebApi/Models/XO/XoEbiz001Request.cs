using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class XoEbiz001Request
{
    [JsonPropertyName("IDNO")]
    [Required]
    public required string idNo { get; init; }
}

