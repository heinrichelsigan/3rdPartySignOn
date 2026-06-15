using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSO3rd.Library;
using ThirdPartySignOn.Saml.Data;

namespace ThirdPartySignOn.Saml.Services
{

    /// <summary>
    /// appsettings.json key reader abstraction
    /// </summary>
    public class Saml2SettingsKeyReader : SettingsKeyReader
    {

        /// <summary>
        /// Gets the saml2 section in appsettings.json
        /// </summary>
        /// <param name="configSection">config section name of saml2 section</param>
        /// <returns><see cref="Saml2IdentConfig"/></returns>
        public static SamlIdentConfig? GetJsonSettingsSectionSaml2(string configSection) =>
                            SamlIdentConfig.GetJsonSettingsSectionSaml2(configSection);


        public static string Saml2EnablerRedirectUrl { get => GetKeySetting("Saml2EnablerRedirectUrl"); }



        public static string Saml2AuthGWPath { get => GetKeySetting("Saml2AuthGWPath"); }

        public static string Saml2CookieName { get => GetKeySetting("Saml2CookieName"); }

        protected internal static string _filesUploadPath = "";

        public static string FilesUploadPath
        {
            get
            {
                if ((_filesUploadPath = GetKeySetting("FilesUploadPath")) != null && !string.IsNullOrEmpty(_filesUploadPath))
                {
                    if (!Directory.Exists(_filesUploadPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(_filesUploadPath);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to create directory for FilesUploadPath: {_filesUploadPath}. Error: {ex.Message}", ex);
                        }
                    }
                    return _filesUploadPath;
                }
                return "";
            }
        }


    }

}
