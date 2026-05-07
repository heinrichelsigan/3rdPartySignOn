using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ThirdPartySignOn.Google.Data
{

    /// <summary>
    /// GoogleOpenIdConfig contains all settings for GoogleOpenIdConfig
    /// </summary>
    [Serializable]
    public class GoogleOpenIdConfig
    {
             
        public string AuthUri { get; set; } // "https://accounts.google.com/o/oauth2/auth"

        public string TokenUri { get; set; } // "https://oauth2.googleapis.com/token"

        public string ProjectId { get; set; } // "api-project-782994905930"

        public string ClientId { get; set; } // "782994905930-6bv6ep3kqd0a4olf1qjogifb2h6kftlh.apps.googleusercontent.com"

        public string AuthProviderX509CertUrl { get; set; } // "https://www.googleapis.com/oauth2/v1/certs",

        public string SMime { get; set; } 

        internal string ClientSecret { get => CryptExtensions.FromBase64(SMime); set => SMime = CryptExtensions.ToBase64(value); }

        public string CallbackPath { get; set; } // "/signin-oidc"

        public GoogleOpenIdConfig()
        {
            AuthUri = "https://accounts.google.com/o/oauth2/auth";
            TokenUri = "https://oauth2.googleapis.com/token";
            ProjectId = "api-project-782994905930";
            AuthProviderX509CertUrl = "https://www.googleapis.com/oauth2/v1/certs";
            ClientId = "782994905930-6bv6ep3kqd0a4olf1qjogifb2h6kftlh.apps.googleusercontent.com";
            SMime = String.Concat("R09DU1", "BYLU", "VjUW5q", "ZEIxM0Nac", "E0tZV", "BLLXYyNlRQdWt0MTgK");
            CallbackPath = "/signin-oidc";
        }

        public GoogleOpenIdConfig(string jsonSectionName) : this()
        {
            if (string.IsNullOrEmpty(jsonSectionName))
                jsonSectionName = "AzureAd";
            GoogleOpenIdConfig? currentConfig = GetJsonSettingsGoogleOpenId(jsonSectionName);
            if (currentConfig != null)
            {
                ClientId = currentConfig.ClientId;
                ProjectId = currentConfig.ProjectId;
                AuthUri = currentConfig.AuthUri;
                TokenUri = currentConfig.TokenUri;
                AuthProviderX509CertUrl = currentConfig.AuthProviderX509CertUrl;
                SMime = currentConfig.SMime;
                CallbackPath = currentConfig.CallbackPath;  
            }
        }


        public static GoogleOpenIdConfig? GetJsonSettingsGoogleOpenId(string configSection = "Authentication:Google")
        {
            GoogleOpenIdConfig? googleOpenIdConfig = null;
            string configPath = Path.Combine(GoogleSettingsKeyReader.BaseAppPath, "appsettings.json");
            if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
            {
                string jsonSerialized = File.ReadAllText(configPath);
                if (!string.IsNullOrEmpty(jsonSerialized))
                {
                    string jsonConfigSection = configSection.Replace(":", ".");
                    JObject? jobj = (JObject?)JsonConvert.DeserializeObject(jsonSerialized);
                    JToken? jtok = (JToken?)jobj?.SelectToken(jsonConfigSection);
                    string restTokenString = (jtok ?? "").ToString();
                    googleOpenIdConfig = JsonConvert.DeserializeObject<GoogleOpenIdConfig>(restTokenString);
                }
            }
            return googleOpenIdConfig;
        }

    }
}

