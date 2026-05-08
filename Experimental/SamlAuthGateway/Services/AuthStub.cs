using Microsoft.AspNetCore.Components.Authorization;
using SamlAuthGateway.Data;
using System.Security.Claims;

namespace SamlAuthGateway.Services
{

    /// <summary>
    /// a stub to get username and claims
    /// </summary>
    public class AuthStub
    {

        #region Saml2 authentication and saml2 claims
       
        public string UserName = "";

        /// <summary>
        /// Authenticate the user and get the username from the name identifier claim. This method is intended for testing purposes only and should not be used in production.
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns>Auth status message</returns>
        public async Task<string> Authenticate(AuthenticationStateProvider authStateProvider)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                UserName = await GetLoginNameIdentifier(authStateProvider);
                return $"{UserName} is authenticated.";
            }
            else
            {
                return "The user is NOT authenticated.";
            }
        }


        /// <summary>
        /// GetLoginNameIdentifier gets the username from the name identifier claim. This method is intended for testing purposes only and should not be used in production.
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{string}">Task containing username</see></returns>
        public async Task<string> GetLoginNameIdentifier(AuthenticationStateProvider authStateProvider)
        {
            SamlUserInfo usr = await GetSamlUserInfo_Async(authStateProvider);
            UserName = usr.LoginName;
            return UserName;            
        }

        /// <summary>
        /// GetSamlUserInfo_Async
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{Saml2UserInfoReduced}"/></returns>
        public async Task<SamlUserInfo> GetSamlUserInfo_Async(AuthenticationStateProvider authStateProvider)
        {
            AuthenticationState authState = await authStateProvider.GetAuthenticationStateAsync();
            SamlUserInfo saml2User = new SamlUserInfo(authState);
            return saml2User;
        }


        /// <summary>
        /// GetSamlUserInfo
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="SamlUserInfo"/></returns>
        public SamlUserInfo GetSamlUserInfo(AuthenticationStateProvider authStateProvider)
        {
            SamlUserInfo saml2User = new SamlUserInfo(authStateProvider);
            return saml2User;            
        }


        #endregion Saml2 authentication and saml2 claims

    }

}

