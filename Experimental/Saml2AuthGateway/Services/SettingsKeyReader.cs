using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saml2AuthGateway.Data;

namespace Saml2AuthGateway.Services
{

    /// <summary>
    /// appsettings.json key reader abstraction
    /// </summary>
    public class SettingsKeyReader
    {

        #region static pre settings

        private static readonly Lock _lock = new Lock();

        private static string _baseAppPath = "";
        public static string BaseAppPath
        {
            get
            {
                lock (_lock)
                {
                    if (string.IsNullOrEmpty(_baseAppPath))
                    {
                        _baseAppPath = Path.GetDirectoryName(Environment.ProcessPath) ?? AppDomain.CurrentDomain.BaseDirectory;
                        if (!string.IsNullOrEmpty(_baseAppPath) && Directory.Exists(_baseAppPath))
                        {
                            if (File.Exists(Path.Combine(_baseAppPath, "appsettins.json")) &&
                                _baseAppPath.Contains(Path.DirectorySeparatorChar + "bin", StringComparison.OrdinalIgnoreCase))
                            {
                                int idx = _baseAppPath.IndexOf(Path.DirectorySeparatorChar + "bin");
                                string bpath = _baseAppPath.Substring(0, idx);
                                if (!string.IsNullOrEmpty(bpath) && Directory.Exists(bpath) && File.Exists(Path.Combine(bpath, "appsettins.json")))
                                    _baseAppPath = bpath;
                            }
                        }
                    }
                }
                return _baseAppPath;
            }
        }

        public static string LogFilePath
        {
            get
            {
                string logFilePath = GetKeySetting("LogFilePath");
                if (!logFilePath.EndsWith(Path.DirectorySeparatorChar))
                    logFilePath += Path.DirectorySeparatorChar.ToString();
                
                if (!Directory.Exists(logFilePath))
                    Directory.CreateDirectory(logFilePath);
                
                return logFilePath;
            }
        }

        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            // Formatting = Formatting.Indented,
            MaxDepth = 16,
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        #endregion static pre settings

        /// <summary>
        /// Generic method to get any key value from appsettings.json by providing the json key path. It first tries to get the value using ConfigurationBuilder, if it fails, it falls back to manually reading the json file and parsing it.
        /// </summary>
        /// <param name="jsonKeyPath">json key path</param>
        /// <returns><see cref="string">key value</see></returns>
        public static string GetKeySetting(string jsonKeyPath)
        {
            var AppSettingsConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string? returnValue = (AppSettingsConfig != null) ? AppSettingsConfig.GetValue<string>(jsonKeyPath) : GetKeyValueJson(jsonKeyPath);

            return returnValue ?? "";
        }

        /// <summary>
        /// Gets the saml2 section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="Saml2IdentConfig"/></returns>
        public static Saml2IdentConfig? GetJsonSettingsSectionSaml2(string configSection)
        {
            Saml2IdentConfig? saml2IdentConf = null;
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
                    saml2IdentConf = JsonConvert.DeserializeObject<Saml2IdentConfig>(restTokenString);
                }
            }
            return saml2IdentConf;
        }

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

        #region appsettings.json fast key value mappings

        public static string DomainName { get => GetKeySetting("DomainName"); }

        public static string HostDomainName { get => GetKeySetting("ServerDomain"); }

        public static string FormsAuthenticationHttpsUrl { get => GetKeySetting("FormsAuthenticationHttpsUrl"); }        
        public static string Saml2EnablerRedirectUrl { get => GetKeySetting("Saml2EnablerRedirectUrl"); }

        public static string Saml2LogoutUrl { get => GetKeySetting("Saml2LogoutUrl"); }
        
        public static string ApplicationName { get => GetKeySetting("ApplicationName"); }

        public static string Saml2AuthGWPath { get => GetKeySetting("Saml2AuthGWPath"); }
       
        public static string Saml2CookieName { get => GetKeySetting("Saml2CookieName"); }

        public static string SamlSoapWebService { get => GetKeySetting("SamlSoapWebService"); }

        public static string FormsAuthenticationHttpUrl {
            get
            {
                string formsAuthenticationHttpUrl = "";
                try
                {
                    formsAuthenticationHttpUrl = GetKeySetting("FormsAuthenticationHttpUrl");
                }
                catch (Exception)
                {
                    formsAuthenticationHttpUrl = string.Empty;
                }
                return formsAuthenticationHttpUrl;
            }
        }

        public static string SamlSoapWebServiceHttp
        {
            get
            {
                string samlSoapWebServiceHttp = "";
                try
                {
                    samlSoapWebServiceHttp = GetKeySetting("SamlSoapWebServiceHttp");
                }
                catch (Exception)
                {
                    samlSoapWebServiceHttp = string.Empty;
                }
                return samlSoapWebServiceHttp;
            }
        }

        #endregion appsettings.json fast key value mappings

    }

}
