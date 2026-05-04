using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace ThirdPartySignOn.Google.Data
{
    public class OAuthTokenRequest
    {
        public OAuthTokenRequest(string code,
            string clientId,
            string clientSecret,
            string redirectUri,
            string grantType)
        {
            this.Code = code;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.RedirectUri = redirectUri;
            this.GrantType = grantType;
        }

        [JsonPropertyName("code")]
        public string Code { get; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; }

        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; }

        [JsonPropertyName("grant_type")]
        public string GrantType { get; }

        public static IEnumerable<KeyValuePair<string, string>> ToKeyValueURL(OAuthTokenRequest tokenRequest) =>
            tokenRequest.GetType().GetProperties()
                .ToList()
                .Select(p => new KeyValuePair<string, string>(
                    p.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                        .Select(x => (JsonPropertyNameAttribute)x).First().Name,
                    p.GetValue(tokenRequest)?.ToString() ?? string.Empty))
                .ToArray();
    }

    public interface IGoogleConnectService
    {
        public Task<UserInfoResponse> GetUserByCodeAsync(string code);
    }

    public class GoogleConnectService : AuthenticationStateProvider, IGoogleConnectService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private IConfiguration Configuration { get; set; }

        private const string TokenClientName = "TokenClient";

        private static HttpClient TokenClient { get; set; }

        private static Uri UserInfoEndpoint { get; set; }

        private static UserInfoResponse? UserInfo { get; set; }

        private static TokenResponse? Token { get; set; }

        private static DateTime TokenExpirationUtc { get; set; }

        public GoogleConnectService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this._httpClientFactory = httpClientFactory;
            this.Configuration = configuration;

            var wellKnownClient = httpClientFactory.CreateClient("Google");
            var openIdConfig = wellKnownClient.GetAsync("Google").Result;

            var readOpenIdConfig = openIdConfig.Content.ReadAsStringAsync().Result;
            var deserializedOpenIdConfig = JsonConvert.DeserializeObject<OpenIdConfiguration>(readOpenIdConfig);
            GoogleOpenIdConfig gcfg = SettingsKeyReader.GetJsonSettingsGoogleOpenId();


            UserInfoEndpoint = new Uri(deserializedOpenIdConfig.UserinfoEndpoint);

            TokenClient = httpClientFactory.CreateClient(TokenClientName);
            TokenClient.BaseAddress = new Uri(deserializedOpenIdConfig.TokenEndpoint);
        }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            AuthenticationState? state = null;
            return state;
        }

        public async Task<UserInfoResponse> GetUserByCodeAsync(string code)
        {
            if (TokenExpirationUtc > DateTime.UtcNow && UserInfo != null)
            {
                return UserInfo;
            }
            GoogleOpenIdConfig gcfg = SettingsKeyReader.GetJsonSettingsGoogleOpenId();


            var tokenRequest = new OAuthTokenRequest(code,

                gcfg.ClientId,
                gcfg.ClientSecret,
                SettingsKeyReader.GoogleRedirectUrl,
                "*");                

            var content = new FormUrlEncodedContent(OAuthTokenRequest.ToKeyValueURL(tokenRequest));

            var result = await TokenClient.PostAsync("", content);

            Token = await result.Content.ReadFromJsonAsync<TokenResponse>();

            TokenExpirationUtc = DateTime.UtcNow.AddSeconds(Token.ExpiresIn);

            var userInfoClient = this._httpClientFactory.CreateClient("newClient");
            userInfoClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            userInfoClient.BaseAddress = UserInfoEndpoint;

            var userInfo = await userInfoClient.GetAsync("");

            UserInfo = await userInfo.Content.ReadFromJsonAsync<UserInfoResponse>();

            return UserInfo;
        }

    }
}
