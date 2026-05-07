using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ThirdPartySignOn.MSIdentity.Data
{

    /// <summary>
    /// AzureOpenIdConfig contains all settings for AzureOpenIdConfig
    /// </summary>
    [Serializable]
    public class AzureOpenIdConfig
    {
             
        public string Instance { get; set; } // login instance "https://login.microsoftonline.com/";

        public string Domain { get; set; } // azure active directory domain "heinrichelsiganlive355.onmicrosoft.com";

        public string TenantId { get; set; } // TenantId from Azure Entra Id "d661f4ad-5daa-4767-b740-084d20d8365f";

        public string ClientId { get; set; } // ClientId from Apps MSIdentity "65eb80ed-ba91-464a-b10e-87ff8a349f32";

        public string CallbackPath { get; set; } // local signin path


        public AzureOpenIdConfig()
        {
            Instance = "https://login.microsoftonline.com/";
            Domain = "heinrichelsiganlive355.onmicrosoft.com";
            TenantId = "d661f4ad-5daa-4767-b740-084d20d8365f";
            ClientId = "65eb80ed-ba91-464a-b10e-87ff8a349f32";
            CallbackPath = "/signin-oidc";
        }

        public AzureOpenIdConfig(string jsonSectionName) : this()
        {
            if (string.IsNullOrEmpty(jsonSectionName))
                jsonSectionName = "AzureAd";
            AzureOpenIdConfig? currentConfig = GetJsonSettingsAzureOpenId(jsonSectionName);
            if (currentConfig != null)
            {
                Instance = currentConfig.Instance;
                Domain = currentConfig.Domain;
                TenantId = currentConfig.TenantId;
                ClientId = currentConfig.ClientId;
                CallbackPath = currentConfig.CallbackPath;
            }
        }


        public static AzureOpenIdConfig? GetJsonSettingsAzureOpenId(string configSection = "AzureAd")
        {
            AzureOpenIdConfig? azureOpenIdConfig = null;
            string configPath = Path.Combine(AzureADSettingsKeyReader.BaseAppPath, "appsettings.json");
            if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
            {
                string jsonSerialized = File.ReadAllText(configPath);
                if (!string.IsNullOrEmpty(jsonSerialized))
                {
                    string jsonConfigSection = configSection.Replace(":", ".");
                    JObject? jobj = (JObject?)JsonConvert.DeserializeObject(jsonSerialized);
                    JToken? jtok = (JToken?)jobj?.SelectToken(jsonConfigSection);
                    string restTokenString = (jtok ?? "").ToString();
                    azureOpenIdConfig = JsonConvert.DeserializeObject<AzureOpenIdConfig>(restTokenString);
                }
            }
            return azureOpenIdConfig;
        }

    }
}

