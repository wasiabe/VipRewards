namespace VipRewardsWebApi.Options;

public sealed class OneTimeKeyOptions
{
    public int KeyTtlSeconds { get; set; } = 120;
    public int RsaKeySize { get; set; } = 2048;
}
