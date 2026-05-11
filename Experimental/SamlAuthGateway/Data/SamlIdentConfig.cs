using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SamlAuthGateway.Services;

namespace SamlAuthGateway.Data
{

    /// <summary>
    /// Saml2IdentConfig contains all settings for SAML2 Identity Provider
    /// </summary>
    [Serializable]
    public class SamlIdentConfig
    {
        public string EntityId { get; set; } = "";

        public string LogoutLocation { get; set; } = "";

        [Serializable]
        public struct IdentProvider
        {
            public IdentProvider()
            {
                EntityId = "";
                MetadataLocation = "";
            }
            public string EntityId { get; set; } = "";
            public string MetadataLocation { get; set; } = "";

            public string LogoutUrl { get; set; } = "";
        }


        public IdentProvider IdentityProvider { get; set; }



        public SamlIdentConfig()
        {
            this.EntityId = (new SettingsKeyReader()).GetKeySetting("Saml2:EntityId");
            this.LogoutLocation = "https://stubidp.sustainsys.com/Logout";
            this.IdentityProvider = new IdentProvider()
            {
                EntityId = "https://stubidp.sustainsys.com/Metadata",
                MetadataLocation = "https://stubidp.sustainsys.com/Metadata",
                LogoutUrl = "https://stubidp.sustainsys.com/Logout"
            };
        }

        public SamlIdentConfig(string configSection) : this()
        {
            SamlIdentConfig? saml2Config = GetJsonSettingsSectionSaml2(configSection);
            if (saml2Config != null)
            {
                this.EntityId = string.IsNullOrEmpty(saml2Config.EntityId) ?
                    (new SettingsKeyReader()).GetKeySetting("Saml2:EntityId") : saml2Config.EntityId;
                this.LogoutLocation = saml2Config.LogoutLocation;
                this.IdentityProvider = new IdentProvider()
                {
                    EntityId = saml2Config.IdentityProvider.EntityId,
                    MetadataLocation = saml2Config.IdentityProvider.MetadataLocation,
                    LogoutUrl = saml2Config.IdentityProvider.LogoutUrl
                };
            }
        }

        /// <summary>
        /// Gets the saml2 section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="SamlIdentConfig"/></returns>
        public static SamlIdentConfig? GetJsonSettingsSectionSaml2(string configSection = "Saml2")
        {
            SamlIdentConfig? saml2IdentConf = null;
            string configPath = Path.Combine(SettingsKeyReader.BaseAppPath, "appsettings.json");
            if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
            {
                string jsonSerialized = File.ReadAllText(configPath);
                if (!string.IsNullOrEmpty(jsonSerialized))
                {
                    string jsonConfigSection = configSection.Replace(":", ".");
                    JObject? jobj = (JObject?)JsonConvert.DeserializeObject(jsonSerialized);
                    JToken? jtok = (JToken?)jobj?.SelectToken(jsonConfigSection);
                    string restTokenString = (jtok ?? "").ToString();
                    saml2IdentConf = JsonConvert.DeserializeObject<SamlIdentConfig>(restTokenString);
                }
            }
            return saml2IdentConf;
        }

    }

}
