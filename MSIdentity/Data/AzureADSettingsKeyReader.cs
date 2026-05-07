using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThirdPartySignOn.MSIdentity.Data;

namespace ThirdPartySignOn.MSIdentity.Data
{

    /// <summary>
    /// appsettings.json key reader abstraction
    /// </summary>
    public class AzureADSettingsKeyReader : SSO3rd.Library.SettingsKeyReader
    {
    

        /// <summary>
        /// GetJsonSettingsAzureOpenId - gets the azure openid section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="AzureOpenIdConfig"/></returns>
        public static AzureOpenIdConfig? GetJsonSettingsAzureOpenId(string configSection = "AzureAd") => 
            AzureOpenIdConfig.GetJsonSettingsAzureOpenId(configSection);

        public static string GetKeyValueJson(string keyPath)
        {
            string jsonFile = Path.Combine(BaseAppPath, "appsettings.json");
            if (File.Exists(jsonFile))
            {
                string jsonSerialized = File.ReadAllText(jsonFile);
                if (!string.IsNullOrEmpty(jsonSerialized))
                {
                    string jsonKeyPath = keyPath.Replace(":", ".");
                    JObject? jobj = (JObject?)JsonConvert.DeserializeObject(jsonSerialized);
                    JToken? jtok = jobj?.SelectToken(jsonKeyPath);
                    if (jtok != null)
                        return jtok.ToString();
                }
            }
            return "";
        }


        public static string AzureGatewayPath { get => GetKeySetting("AzureGatewayPath"); }
       
        public static string AzureLogFilePath { get => GetKeySetting("LogFilePath"); }


    }

}
