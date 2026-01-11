using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// ========== 設定讀取 ==========
var settings = LoadSettings();
var baseUrl = settings.Api.BaseUrl.TrimEnd('/');
var plainText = settings.TestData.PlainText;

// requestId：兩支 API 必須一致
var requestId = Guid.NewGuid().ToString();

// ========== HttpClient ==========
using var http = CreateHttpClient(baseUrl);

// ========== 1) 呼叫 GetOneTimePublicKey 取得 PEM 公鑰 ==========
Console.WriteLine("== Step 1: GetOneTimePublicKey ==");

var pubResp = await PostJsonAsync<GetOneTimePublicKeyResponse>(
    http,
    "/api/VipRewards/GetOneTimePublicKey",
    new GetOneTimePublicKeyRequest { RequestId = requestId });

Console.WriteLine("requestId:");
Console.WriteLine(requestId);
Console.WriteLine();
Console.WriteLine("publicKey (PEM, first 120 chars):");
Console.WriteLine(pubResp.PublicKey.Length > 120 ? pubResp.PublicKey[..120] + "..." : pubResp.PublicKey);
Console.WriteLine();

// ========== 2) 客戶端端加密（Hybrid：RSA 加 AES key、AES-GCM 加資料） ==========
Console.WriteLine("== Step 2: Encrypt (RSA-OAEP + AES-GCM) ==");

// 2-1) 產生 AES-256 key（32 bytes） + 12 bytes IV（GCM 建議 12 bytes）
byte[] aesKey = RandomNumberGenerator.GetBytes(32);
byte[] iv = RandomNumberGenerator.GetBytes(12);

// 2-2) AES-GCM 加密 plaintext -> cipherText + tag
byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
byte[] cipherText = new byte[plainBytes.Length];
byte[] tag = new byte[16]; // 常見 16 bytes tag

using (var aes = new AesGcm(aesKey))
{
    aes.Encrypt(iv, plainBytes, cipherText, tag);
}

// 2-3) 匯入 PEM 公鑰，RSA-OAEP(SHA-256) 加密 AES key
using RSA rsaPub = RSA.Create();
rsaPub.ImportFromPem(pubResp.PublicKey);

byte[] encryptedKey = rsaPub.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);

// 2-4) 轉 Base64 組成 SendEncryptedData Request
var sendReq = new GetVipInfoRequest
{
    RequestId = requestId,
    EncryptedKey = Convert.ToBase64String(encryptedKey),
    Iv = Convert.ToBase64String(iv),
    CipherText = Convert.ToBase64String(cipherText),
    Tag = Convert.ToBase64String(tag)
};

Console.WriteLine("PlainText:");
Console.WriteLine(plainText);
Console.WriteLine();
Console.WriteLine("Encrypted payload sizes:");
Console.WriteLine($"encryptedKey: {encryptedKey.Length} bytes (base64 len {sendReq.EncryptedKey.Length})");
Console.WriteLine($"iv         : {iv.Length} bytes (base64 len {sendReq.Iv.Length})");
Console.WriteLine($"cipherText : {cipherText.Length} bytes (base64 len {sendReq.CipherText.Length})");
Console.WriteLine($"tag        : {tag.Length} bytes (base64 len {sendReq.Tag.Length})");
Console.WriteLine();

// ========== 3) 呼叫 SendEncryptedData ==========
Console.WriteLine("== Step 3: SendEncryptedData ==");

var decResp = await PostJsonAsync<ApiResponse<GetVipInfoDataResponse>>(
    http,
    "/api/VipRewards/GetVipInfo",
    sendReq);

Console.WriteLine("decryptedData:");
Console.WriteLine(decResp.Data.Id);
Console.WriteLine();

// ========== 4) 驗證 ==========
Console.WriteLine("== Step 4: Verify ==");
Console.WriteLine(decResp.Data.Id == "A123XXX789" ? "OK ✅ (decryptedData == plainText)" : "FAILED ❌");
Console.WriteLine("Done.");

// =================== 以下是程式內部 helper / DTO ===================

static HttpClient CreateHttpClient(string baseUrl)
{
    // 開發環境常見：用 dev cert / 自簽憑證時會打不過 TLS
    // 你如果確定是 HTTPS 且憑證 OK，可以把這段「忽略憑證」刪掉。
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return new HttpClient(handler)
    {
        BaseAddress = new Uri(baseUrl),
        Timeout = TimeSpan.FromSeconds(30)
    };
}

static async Task<T> PostJsonAsync<T>(HttpClient http, string path, object body)
{
    using var resp = await http.PostAsJsonAsync(path, body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    var content = await resp.Content.ReadAsStringAsync();

    if (!resp.IsSuccessStatusCode)
    {
        throw new HttpRequestException(
            $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}\nPath: {path}\nBody: {content}");
    }

    var obj = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    if (obj is null) throw new InvalidOperationException("Response JSON deserialize failed.");

    return obj;
}

static AppSettings LoadSettings()
{
    var json = File.ReadAllText("appsettings.json");
    var settings = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    if (settings is null) throw new InvalidOperationException("Failed to load appsettings.json");
    return settings;
}

// ---------- Settings ----------
sealed class AppSettings
{
    public ApiSettings Api { get; set; } = new();
    public TestDataSettings TestData { get; set; } = new();
}

sealed class ApiSettings
{
    public string BaseUrl { get; set; } = "https://localhost:5001";
}

sealed class TestDataSettings
{
    public string PlainText { get; set; } = "{\"hello\":\"world\"}";
}

// ---------- API DTO ----------
sealed class GetOneTimePublicKeyRequest
{
    public string RequestId { get; set; } = default!;
}

sealed class GetOneTimePublicKeyResponse
{
    public string PublicKey { get; set; } = default!;
}

sealed class GetVipInfoRequest
{
    public string RequestId { get; set; } = default!;
    public string EncryptedKey { get; set; } = default!;
    public string Iv { get; set; } = default!;
    public string CipherText { get; set; } = default!;
    public string Tag { get; set; } = default!;
}

sealed class ApiResponse<T>
{
    public string Code { get; set; } = default!;
    public string Message { get; set; }
    public T Data { get; set; }
}

sealed class GetVipInfoDataResponse
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
