using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSO3rd.Library;
using ThirdPartySignOn.Saml.Services;

namespace ThirdPartySignOn.Saml.Data
{

    /// <summary>
    /// SamlIdentConfig contains all settings for SAML2 Identity Provider
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
            this.EntityId = SettingsKeyReader.GetKeySetting("Saml2:EntityId");
            this.LogoutLocation = "https://stubidp.sustainsys.com/Logout";
            this.IdentityProvider = new IdentProvider()
            {
                EntityId = "https://stubidp.sustainsys.com/Metadata",
                MetadataLocation = "https://stubidp.sustainsys.com/Metadata",
                LogoutUrl = "https://stubidp.sustainsys.com/Logout"
            };
        }

        public SamlIdentConfig(string jsonSectionId) : this()
        {
            SamlIdentConfig? samlConfig = GetJsonSettingsSectionSaml2(jsonSectionId);
            if (samlConfig != null)
            {
                this.EntityId = string.IsNullOrEmpty(samlConfig.EntityId) ?
                                    Saml2SettingsKeyReader.GetKeySetting("Saml2:EntityId") :
                                    samlConfig.EntityId;
                this.LogoutLocation = string.IsNullOrEmpty(samlConfig.LogoutLocation) ?
                                        Saml2SettingsKeyReader.GetKeySetting("Saml2:LogoutLocatíon") :
                                        samlConfig.LogoutLocation;
                this.IdentityProvider = new IdentProvider()
                {
                    EntityId = string.IsNullOrEmpty(samlConfig.IdentityProvider.EntityId) ?
                                        Saml2SettingsKeyReader.GetKeySetting("Saml2:IdentityProvider:EntityId") :
                                        samlConfig.IdentityProvider.EntityId,
                    MetadataLocation = string.IsNullOrEmpty(samlConfig.IdentityProvider.MetadataLocation) ?
                                        Saml2SettingsKeyReader.GetKeySetting("Saml2:IdentityProvider:MetadataLocation") :
                                        samlConfig.IdentityProvider.MetadataLocation,
                    LogoutUrl = string.IsNullOrEmpty(samlConfig.IdentityProvider.LogoutUrl) ?
                                        Saml2SettingsKeyReader.GetKeySetting("Saml2:IdentityProvider:LogoutUrl") :
                                        samlConfig.IdentityProvider.LogoutUrl
                };
            }
        }

        /// <summary>
        /// Gets the saml2 section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="Saml2IdentConfig"/></returns>
        public static SamlIdentConfig? GetJsonSettingsSectionSaml2(string configSection = "Saml2")
        {
            SamlIdentConfig? samlIdentConf = null;
            string configPath = Path.Combine(Saml2SettingsKeyReader.BaseAppPath, "appsettings.json");
            if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
            {
                string jsonSerialized = File.ReadAllText(configPath);
                if (!string.IsNullOrEmpty(jsonSerialized))
                {
                    string jsonConfigSection = configSection.Replace(":", ".");
                    JObject? jobj = (JObject?)JsonConvert.DeserializeObject(jsonSerialized);
                    JToken? jtok = (JToken?)jobj?.SelectToken(jsonConfigSection);
                    string restTokenString = (jtok ?? "").ToString();
                    samlIdentConf = JsonConvert.DeserializeObject<SamlIdentConfig>(restTokenString);
                }
            }
            return samlIdentConf;
        }

    }

}
