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

    }

}
