using Saml2AuthGateway.Data;
using Saml2AuthGateway.Services;
using Saml2AuthGateway.Services.Saml2Net48;

namespace Saml2AuthGateway.Services
{

    /// <summary>
    /// Saml2SoapClient is a WebService consuming stub to Reference Saml2Net48ServiceReference
    /// local url https://localhost/Saml2/Saml2Net48/Saml2Service.asmx
    /// prod url https://test.enabler.info/current/Saml2Net48/Saml2Service.asmx
    /// </summary>
    public class Saml2SoapClient
    {

        internal static Saml2ServiceSoapClient client = new Saml2ServiceSoapClient(
            Saml2ServiceSoapClient.EndpointConfiguration.Saml2ServiceSoap12,
            SettingsKeyReader.SamlSoapWebService);


        internal static Saml2ServiceSoapClient clientHttp = new Saml2ServiceSoapClient(
            Saml2ServiceSoapClient.EndpointConfiguration.Saml2ServiceSoap11,
            SettingsKeyReader.SamlSoapWebServiceHttp);

        /// <summary>
        /// Empty default ctor
        /// </summary>
        static Saml2SoapClient()
        {
        }

        #region Saml2 authentication and saml2 claims

        /// <summary>
        /// GetFormsTicketAsync async call to get formsauth ticket for user 
        /// </summary>
        /// <param name="saml2UserName">plain or encrypted username</param>
        /// <param name="isEncrypted">true, if username is already encrypted</param>
        /// <returns>formsauthticket</returns>
        public static async Task<string> GetFormsTicketAsync(string saml2UserName, bool isEncrypted = false)
        {
            string formAuthTicket = string.Empty;
            try
            {
                EnablerLog.LogOriginMsg("Saml2SoapClient.GetFormsTicketAsync",
                    $"saml2UserName = {saml2UserName}, isEncrypted = {isEncrypted}");
                string saml2EncryptedUserName = (isEncrypted) ? saml2UserName : saml2UserName.EnCrypt();
                var response = await client.GetFormAuthTicketForSaml2UserAsync(saml2EncryptedUserName, false);
                if (response != null)
                {
                    formAuthTicket = response.Body.GetFormAuthTicketForSaml2UserResult;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                EnablerLog.LogOriginMsgEx("Saml2SoapClient.GetFormsTicketAsync", 
                    $"Error getting forms ticket for user {saml2UserName};\t\n{client.Endpoint.ToString()}", ex);
                formAuthTicket = "";              
            }

            return formAuthTicket;
        }

        /// <summary>
        /// GetFormsTicket synchronous call of Webservice  to get formsauth ticket for user 
        /// </summary>
        /// <param name="saml2UserName">plain or encrypted username</param>
        /// <param name="isEncrypted">true, if username is already encrypted</param>
        /// <returns>formsauthticket</returns>
        public static string GetFormsTicket(string saml2UserName, bool isEncrypted = false)
        {
            string formAuthTicket = string.Empty;
            try
            {
                EnablerLog.LogOriginMsg("Saml2SoapClient.GetFormsTicket",
                    $"saml2UserName = {saml2UserName}, isEncrypted = {isEncrypted}");
                string saml2EncryptedUserName = (isEncrypted) ? saml2UserName : saml2UserName.EnCrypt();
                var response = client.GetFormAuthTicketForSaml2User(saml2EncryptedUserName, false);
                if (response != null)
                {
                    formAuthTicket = response;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                EnablerLog.LogOriginMsgEx("Saml2SoapClient.GetFormsTicket", 
                    $"Error getting forms ticket for user {saml2UserName};\t\n{client.Endpoint.ToString()}", ex);
                formAuthTicket = "";                 
            }
            return formAuthTicket;
        }


        /// <summary>
        /// RegisterSaml2UserAsync is to register a new Saml2 login user,
        /// so that the Saml2Net48 knows, that he is legal to redirect
        /// </summary>
        /// <param name="saml2UserName">saml2 user oder user name encrypted</param>
        /// <param name="isEncrypted">true, if username is already encrypted</param>
        /// <returns>true, if registration  was successfully</returns>
        public static bool RegisterSaml2User(string saml2UserName, bool isEncrypted = false)
        {
            bool isRegistered = false;
            try
            {
                EnablerLog.LogOriginMsg("Saml2SoapClient.RegisterSaml2User",
                    $"saml2UserName = {saml2UserName}, isEncrypted = {isEncrypted}");
                string saml2EncryptedUserName = (isEncrypted) ? saml2UserName : saml2UserName.EnCrypt();
                var response = client.RegisterEncryptedUserToRedirect(saml2EncryptedUserName);
                isRegistered = response;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                EnablerLog.LogOriginMsgEx("Saml2SoapClient.RegisterSaml2User", 
                    $"Error registering saml2 user {saml2UserName};\t\n{client.Endpoint.ToString()}", ex);
                isRegistered = false;
            }

            return isRegistered;
        }

        /// <summary>
        /// RegisterSaml2UserAsync is to register a new Saml2 login user,
        /// so that the Saml2Net48 knows, that he is legal to redirect
        /// </summary>
        /// <param name="saml2UserName">saml2 user oder user name encrypted</param>
        /// <param name="isEncrypted">true, if username is already encrypted</param>
        /// <returns>true, if registration  was successfully</returns>
        public static async Task<bool> RegisterSaml2UserAsync(string saml2UserName, bool isEncrypted = false)
        {
            bool isRegistered = false;
            try
            {
                EnablerLog.LogOriginMsg("Saml2SoapClient.RegisterSaml2UserAsync",
                   $"saml2UserName = {saml2UserName}, isEncrypted = {isEncrypted}");
                string saml2EncryptedUserName = (isEncrypted) ? saml2UserName : saml2UserName.EnCrypt();
                var response = await client.RegisterEncryptedUserToRedirectAsync(saml2EncryptedUserName);
                isRegistered = response.Body.RegisterEncryptedUserToRedirectResult;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                EnablerLog.LogOriginMsgEx("Saml2SoapClient.RegisterSaml2UserAsync",
                    $"Error registering saml2 user {saml2UserName};\t\n{client.Endpoint.ToString()}", ex);
                isRegistered = false;                
            }

            return isRegistered;
        }

        #endregion Saml2 authentication and saml2 claims


        #region http calls only

        public static string GetFormsTicketHttp(string saml2UserName, bool isEncrypted = false)
        {
            string formAuthTicket = string.Empty;
            try
            {
                EnablerLog.LogOriginMsg("Saml2SoapClient.GetFormsTicketHttp",
                    $"saml2UserName = {saml2UserName}, isEncrypted = {isEncrypted}");
                string saml2EncryptedUserName = (isEncrypted) ? saml2UserName : saml2UserName.EnCrypt();
                var response = clientHttp.GetFormAuthTicketForSaml2User(saml2EncryptedUserName, false);
                if (response != null)
                {
                    formAuthTicket = response;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                EnablerLog.LogOriginMsgEx("Saml2SoapClient.GetFormsTicketHttp", 
                    $"Error getting forms ticket for user {saml2UserName};\t\n{clientHttp.Endpoint.ToString()}", ex);
                formAuthTicket = ex.Message;
            }
            return formAuthTicket;
        }

        public static async Task<string> GetFormsTicketHttpAsync(string saml2UserName, bool isEncrypted = false)
        {
            string formAuthTicket = string.Empty;
            try
            {
                EnablerLog.LogOriginMsg("Saml2SoapClient.GetFormsTicketHttpAsync",
                    $"saml2UserName = {saml2UserName}, isEncrypted = {isEncrypted}");
                string saml2EncryptedUserName = (isEncrypted) ? saml2UserName : saml2UserName.EnCrypt();
                var response = await clientHttp.GetFormAuthTicketForSaml2UserAsync(saml2EncryptedUserName, false);
                if (response != null)
                {
                    formAuthTicket = response.Body.GetFormAuthTicketForSaml2UserResult;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                EnablerLog.LogOriginMsgEx("Saml2SoapClient.GetFormsTicketAsyncHttp",
                    $"Error getting forms ticket for user {saml2UserName};\t\n{clientHttp.Endpoint.ToString()}", ex);
                formAuthTicket = ex.Message;
            }

            return formAuthTicket;
        }

        public static bool RegisterSaml2UserHttp(string saml2UserName, bool isEncrypted = false)
        {
            bool isRegistered = false;
            try
            {
                EnablerLog.LogOriginMsg("Saml2SoapClient.RegisterSaml2UserHttp",
                    $"saml2UserName = {saml2UserName}, isEncrypted = {isEncrypted}");
                string saml2EncryptedUserName = (isEncrypted) ? saml2UserName : saml2UserName.EnCrypt();
                var response = clientHttp.RegisterEncryptedUserToRedirect(saml2EncryptedUserName);
                isRegistered = response;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                EnablerLog.LogOriginMsgEx("Saml2SoapClient.RegisterSaml2UserHttp", 
                    $"Error registering saml2 user {saml2UserName};\t\n{clientHttp.Endpoint.ToString()}", ex);
                isRegistered = false;
                throw;
            }

            return isRegistered;
        }

        public static async Task<bool> RegisterSaml2UserHttpAsync(string saml2UserName, bool isEncrypted = false)
        {
            bool isRegistered = false;
            try
            {
                EnablerLog.LogOriginMsg("Saml2SoapClient.RegisterSaml2UserHttpAsync",
                   $"saml2UserName = {saml2UserName}, isEncrypted = {isEncrypted}");
                string saml2EncryptedUserName = (isEncrypted) ? saml2UserName : saml2UserName.EnCrypt();
                var response = await clientHttp.RegisterEncryptedUserToRedirectAsync(saml2EncryptedUserName);
                isRegistered = response.Body.RegisterEncryptedUserToRedirectResult;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                EnablerLog.LogOriginMsgEx("Saml2SoapClient.RegisterSaml2UserHttpAsync", 
                    $"Error registering saml2 user {saml2UserName};\t\n{clientHttp.Endpoint.ToString()}", ex);
                isRegistered = false;
                throw;
            }

            return isRegistered;
        }


        #endregion http calls only
    }

}

