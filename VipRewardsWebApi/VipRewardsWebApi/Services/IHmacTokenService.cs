using VipRewardsWebApi.Models;

namespace VipRewardsWebApi.Services;

public interface IHmacTokenService
{
    string IssueToken(string plainText);
    string RedeemToken(string token);
}
