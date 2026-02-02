using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text.Json;
using VipRewardsWebApi.Models;
using VipRewardsWebApi.Options;
using VipRewardsWebApi.Services;
using WebApiTemplate.Controllers;

namespace VipRewardsWebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class VipRewardsController : AppControllerBase
    {
        private readonly ILogger<DemoController> _logger;
        private readonly IOneTimeKeyStore _keyStore;
        private readonly RsaCryptoService _rsa;
        private readonly AesGcmCryptoService _aes;
        private readonly OneTimeKeyOptions _opt;
        private readonly IHmacTokenService _hmacService;
        private readonly IXoInParamRepository _xoInParamRepository;
        private readonly VipRewardService _vipRewardService;
        public VipRewardsController(
            ILogger<DemoController> logger,
            IOneTimeKeyStore keyStore,
            RsaCryptoService rsa,
            AesGcmCryptoService aes,
            IOptions<OneTimeKeyOptions> opt,
            IHmacTokenService hmacService,
            IXoInParamRepository xoInParamRepository,
            VipRewardService vipRewardService)
        {
            _logger = logger;
            _keyStore = keyStore;
            _rsa = rsa;
            _aes = aes;
            _opt = opt.Value;
            _hmacService = hmacService;
            _xoInParamRepository = xoInParamRepository;
            _vipRewardService = vipRewardService;
        }

        /// <summary>
        /// API-VIP003 產生保戶資訊加密一次性公鑰
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("GetOneTimePublicKey")]
        public ActionResult<ApiResponse<GetOneTimePublicKeyResponse>> GetOneTimePublicKey([FromBody] GetOneTimePublicKeyRequest req)
        {
            if (!Guid.TryParse(req.RequestId, out _))
                return BadRequest(Error.Validation("requestId must be a UUID string."));

            var rsa = RSA.Create(_opt.RsaKeySize);

            _keyStore.Put(req.RequestId, rsa, TimeSpan.FromSeconds(_opt.KeyTtlSeconds));

            return Ok(new GetOneTimePublicKeyResponse
            {
                PublicKey = _rsa.ExportPublicKeyPem(rsa)
            });
        }

        /// <summary>
        /// API-VIP001 保戶VIP資訊查詢
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("GetVipInfo")]
        public ActionResult<ApiResponse<GetVipInfoResponse>> GetVipInfo([FromBody] GetVipInfoRequest req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Tk))
                return BadRequest("Token is required.");

            string plainText =_hmacService.RedeemToken(req.Tk);

            VipInfo vipInfo = new VipInfo
            {
                VipInfoId = Guid.NewGuid().ToString("N"),
                Name = "何X仁",
                VipLevel = "1",
                ValidFrom = "2026/01/18",
                ValidTill = "2027/01/17",
                RewardPoints = 1688,
            };
            GetVipInfoResponse getVipInfoResponse = new GetVipInfoResponse();
            getVipInfoResponse.VipInfos = new List<VipInfo>();
            getVipInfoResponse.VipInfos?.Add(vipInfo);

            // 使用靜態 Success 方法建立回應物件
            var resp = ApiResponse<GetVipInfoResponse>.Success(getVipInfoResponse);

            return Ok(resp);

        }

        /// <summary>
        /// API-VIP002 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("ValidatePolicyOwner")]
        public async Task<ActionResult<ApiResponse<ValidatePolicyOwnerResponse>>> ValidatePolicyOwner([FromBody] ValidatePolicyOwnerRequest req)
        {
            if (!Guid.TryParse(req.RequestId, out _))
                return BadRequest(Error.Validation("requestId must be a UUID string."));

            if (string.IsNullOrWhiteSpace(req.EncryptedKey) ||
                string.IsNullOrWhiteSpace(req.Iv) ||
                string.IsNullOrWhiteSpace(req.CipherText) ||
                string.IsNullOrWhiteSpace(req.Tag))
            {
                return BadRequest(Error.Validation("encryptedKey, iv, cipherText, tag are required."));
            }

            var rsa = _keyStore.Take(req.RequestId);
            if (rsa is null)
                return NotFound(Error.Validation("One-time private key not found (expired or already used)."));

            try
            {
                var encryptedKeyBytes = RsaCryptoService.FromBase64(req.EncryptedKey, "encryptedKey");
                var iv = RsaCryptoService.FromBase64(req.Iv, "iv");
                var cipherText = RsaCryptoService.FromBase64(req.CipherText, "cipherText");
                var tag = RsaCryptoService.FromBase64(req.Tag, "tag");

                // 1) RSA 解 AES key
                var aesKey = _rsa.DecryptOaepSha256(rsa, encryptedKeyBytes);

                // 基本防呆：AES key 長度應為 16/24/32
                if (aesKey.Length is not (16 or 24 or 32))
                    return BadRequest(Error.Validation($"Decrypted AES key length invalid: {aesKey.Length} bytes."));

                // 2) AES-GCM 解密資料
                var plain = _aes.DecryptToUtf8(aesKey, iv, cipherText, tag);

                // 3) 將 plain 當作 JSON 解析，並取出 id/name
                string id;
                string birthDate;
                try
                {
                    using var doc = JsonDocument.Parse(plain);
                    var root = doc.RootElement;

                    if (!root.TryGetProperty("id", out var idProp) || idProp.ValueKind != JsonValueKind.String)
                        return BadRequest(Error.Validation("Decrypted payload must contain string property 'id'."));

                    if (!root.TryGetProperty("birthDate", out var birthDateProp) || birthDateProp.ValueKind != JsonValueKind.String)
                        return BadRequest(Error.Validation("Decrypted payload must contain string property 'name'."));

                    id = idProp.GetString()!;
                    birthDate = birthDateProp.GetString()!;
                }
                catch (JsonException)
                {
                    return BadRequest(Error.Validation("Decrypted payload is not valid JSON."));
                }

                _ = await _vipRewardService.GetXODataAsync(
                    "EBIZ001",
                    new { id, birthDate },
                    HttpContext.RequestAborted);

                // 4) 將id作HMAC256 Hash
                var tk = _hmacService.IssueToken(id);

                // 5) 建立回應物件
                VipInfoQueryToken vipInfoToken = new VipInfoQueryToken
                {
                    Tk = tk,
                };
                ValidatePolicyOwnerResponse validateVipInfoResponse = new ValidatePolicyOwnerResponse();
                validateVipInfoResponse.Tokens = new List<VipInfoQueryToken>();
                validateVipInfoResponse.Tokens?.Add(vipInfoToken);

                // 使用靜態 Success 方法建立回應物件
                var resp = ApiResponse<ValidatePolicyOwnerResponse>.Success(validateVipInfoResponse);

                return Ok(resp);
            }
            catch (CryptographicException ex)
            {
                return BadRequest(new { message = "Decrypt failed.", detail = ex.Message });
            }
            finally
            {
                rsa.Dispose(); // 一次性私鑰，取出後用完立即 Dispose
            }
        }
    }
}
