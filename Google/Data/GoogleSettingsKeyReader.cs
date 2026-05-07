using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThirdPartySignOn.Google.Data;

namespace ThirdPartySignOn.Google.Data
{

    /// <summary>
    /// appsettings.json key reader abstraction
    /// </summary>
    public class GoogleSettingsKeyReader : SSO3rd.Library.SettingsKeyReader
    {        

        /// <summary>
        /// GetJsonSettingsGoogleOpenId - gets the azure openid section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="GoogleOpenIdConfig"/></returns>
        public static GoogleOpenIdConfig? GetJsonSettingsGoogleOpenId(string configSection = "Authentication:Google") => 
            GoogleOpenIdConfig.GetJsonSettingsGoogleOpenId(configSection);

        
        public static string GoogleGatewayPath { get => GetKeySetting("GoogleGatewayPath"); }
       
        public static string GoogleLogFilePath { get => GetKeySetting("LogFilePath"); }

        

    }

}
