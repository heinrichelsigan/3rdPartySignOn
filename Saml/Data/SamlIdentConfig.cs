using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThirdPartySignOn.Saml.Services;

namespace ThirdPartySignOn.Saml.Data
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
            SamlIdentConfig? saml2Config = GetJsonSettingsSectionSaml2(jsonSectionId);
            if (saml2Config != null)
            {
                this.EntityId = saml2Config.EntityId;
                this.LogoutLocation = saml2Config.LogoutLocation;
                this.IdentityProvider = new IdentProvider()
                {
                    EntityId = saml2Config.IdentityProvider.EntityId,
                    MetadataLocation = saml2Config.IdentityProvider.MetadataLocation,
                    LogoutUrl = saml2Config.IdentityProvider.LogoutUrl
                };
                // this.IdentityProvider = new IdentProvider();
                // this.IdentityProvider.EntityId = saml2Config.IdentityProvider.EntityId;
                // this.IdentityProvider.MetadataLocation = saml2Config.IdentityProvider.MetadataLocation;
                // this.IdentityProvider.LogoutUrl = saml2Config.IdentityProvider.LogoutUrl;
            }
        }

        /// <summary>
        /// Gets the saml2 section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="Saml2IdentConfig"/></returns>
        public static SamlIdentConfig? GetJsonSettingsSectionSaml2(string configSection = "Saml2")
        {
            SamlIdentConfig? saml2IdentConf = null;
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
                    saml2IdentConf = JsonConvert.DeserializeObject<SamlIdentConfig>(restTokenString);
                }
            }

            return saml2IdentConf;
        }

    }

}
