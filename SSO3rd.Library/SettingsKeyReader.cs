
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSO3rd.Library;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace SSO3rd.Library
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
        /// GetKeyValueJson 
        /// </summary>
        /// <param name="keyPath">JPath in jsonfile</param>
        /// <returns>key value</returns>
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

        public static string RedirectUrl { get => GetKeySetting("RedirectUrl"); }

        public static string LogoutUrl { get => GetKeySetting("LogoutUrl"); }

        public static string DomainName { get => GetKeySetting("DomainName"); }

        public static string HostDomainName { get => GetKeySetting("ServerDomain"); }
                

        #endregion appsettings.json fast key value mappings

    }

}

