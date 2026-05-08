using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SamlAuthGateway.Data;

namespace SamlAuthGateway.Services
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


        private static string _logFilePath = "";
        public string LogFilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_logFilePath) && Directory.Exists(_logFilePath))
                    return _logFilePath; 
                
                _logFilePath = GetKeySetting("LogFilePath");
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    _logFilePath = Path.Combine(SettingsKeyReader.BaseAppPath, "Log");
                }
                if (!_logFilePath.EndsWith(Path.DirectorySeparatorChar))
                    _logFilePath += Path.DirectorySeparatorChar.ToString();

                if (!Directory.Exists(_logFilePath))
                    Directory.CreateDirectory(_logFilePath);

                return _logFilePath;
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
        public string GetKeySetting(string jsonKeyPath)
        {
            var AppSettingsConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string? returnValue = (AppSettingsConfig != null) ? AppSettingsConfig.GetValue<string>(jsonKeyPath) : GetKeyValueJson(jsonKeyPath);

            return returnValue ?? "";
        }

        /// <summary>
        /// Gets the saml2 section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="SamlIdentConfig"/></returns>
        public SamlIdentConfig? GetJsonSettingsSectionSaml2(string configSection) => SamlIdentConfig.GetJsonSettingsSectionSaml2(configSection);
        
        public  string GetKeyValueJson(string keyPath)
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

        public string DomainName { get => GetKeySetting("DomainName"); }

        public static string HostDomain { get => (new SettingsKeyReader()).GetKeySetting("DomainName"); }


        public string HostDomainName { get => GetKeySetting("HostDomainName"); }
       
        public string RedirectUrl { get => GetKeySetting("RedirectUrl"); }

        public string LogoutUrl { get => GetKeySetting("LogoutUrl"); }
        
        public string ApplicationName { get => GetKeySetting("ApplicationName"); }

        public static string ApplName { get => (new SettingsKeyReader()).GetKeySetting("ApplicationName"); }


        public string Saml2AuthGWPath { get => GetKeySetting("Saml2AuthGWPath"); }
       
        public string Saml2CookieName { get => GetKeySetting("Saml2CookieName"); }

        public static string SamlCookie { get => (new SettingsKeyReader()).Saml2CookieName; }

        #endregion appsettings.json fast key value mappings

    }

}
