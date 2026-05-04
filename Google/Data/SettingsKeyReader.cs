using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThirdPartySignOn.Google.Data;

namespace ThirdPartySignOn.Google.Data
{

    /// <summary>
    /// appsettings.json key reader abstraction
    /// </summary>
    public class SettingsKeyReader
    {
        
        #region static pre settings

        private static string _baseAppPath = "";
        public static string BaseAppPath
        {
            get
            {
                if (string.IsNullOrEmpty(_baseAppPath))
                {
                    _baseAppPath = Path.GetDirectoryName(Environment.ProcessPath) ?? AppDomain.CurrentDomain.BaseDirectory;
                    if (!string.IsNullOrEmpty(_baseAppPath) && Directory.Exists(_baseAppPath))
                    {
                        if (_baseAppPath.EndsWith(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                            _baseAppPath = _baseAppPath.Substring(0, _baseAppPath.Length - 5);
                        if (_baseAppPath.EndsWith(Path.DirectorySeparatorChar + "bin", StringComparison.OrdinalIgnoreCase))
                            _baseAppPath = _baseAppPath.Substring(0, _baseAppPath.Length - 4);
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
                if (string.IsNullOrEmpty(logFilePath))
                    logFilePath = Path.Combine(BaseAppPath, "Log");
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
        /// GetJsonSettingsAzureOpenId - gets the azure openid section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="AzureOpenIdConfig"/></returns>
        public static GoogleOpenIdConfig? GetJsonSettingsGoogleOpenId(string configSection = "Authentication:Google") => 
            GoogleOpenIdConfig.GetJsonSettingsGoogleOpenId(configSection);

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

        public static string ApplicationName { get => GetKeySetting("ApplicationName"); }

        public static string DomainName { get => GetKeySetting("DomainName"); }

        public static string ServerHostName { get => GetKeySetting("ServerHostName"); }

        public static string GoogleRedirectUrl { get => GetKeySetting("GoogleRedirectUrl"); }

        public static string GoogleLogoutUrl { get => GetKeySetting("GoogleLogoutUrl"); }    

        public static string GoogleGatewayPath { get => GetKeySetting("GoogleGatewayPath"); }
       
        public static string GoogleLogFilePath { get => GetKeySetting("LogFilePath"); }

        #endregion appsettings.json fast key value mappings

    }

}
