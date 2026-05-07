using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Claims;
using System.Security.Claims;
using ThirdPartySignOn.Saml.Data;

namespace ThirdPartySignOn.Saml.Services
{

    /// <summary>
    /// a stub to get username and claims
    /// </summary>
    public class AuthenticateStub
    {

        #region Saml authentication and saml claims

        public string UserName = "";

        /// <summary>
        /// GetLoginNameIdentifier gets the username from the name identifier claim. This method is intended for testing purposes only and should not be used in production.
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{string}">Task containing username</see></returns>
        public async Task<string> GetLoginNameIdentifier(AuthenticationStateProvider authStateProvider)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            UserName = "";
            var user = authState.User;            

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                UserName = user.Identity.Name ?? "";
                foreach (var sclaim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    if (sclaim.Type.Contains("nameidentifier"))
                    {
                        UserName = sclaim.Value;
                        break;
                    }
                }
            }

            return UserName;
        }

        /// <summary>
        /// GetSaml2UserInfoReduced
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{SamlUserInfoReduced}"/></returns>
        public async Task<SamlUserInfo> GetSamlUserInfoReduced(AuthenticationStateProvider authStateProvider)
        {
            SamlUserInfo saml2User = new SamlUserInfo();
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                if (saml2User.ClaimsDictionary == null)
                    saml2User.ClaimsDictionary = new Dictionary<string, string>();
                else
                    saml2User.ClaimsDictionary.Clear();

                foreach (var sclaim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    if (sclaim == null)
                        continue;

                    if (saml2User.ClaimsDictionary.ContainsKey(sclaim.Type))
                        saml2User.ClaimsDictionary[sclaim.Type] = sclaim.Value;
                    else 
                        saml2User.ClaimsDictionary.Add(sclaim.Type, sclaim.Value); // add claim type / value to dictionary

                    if (sclaim.Type.Contains("nameidentifier")) // get name identifier claim value
                        saml2User.NameIdentifier = sclaim.Value;
                    if (sclaim.Type.Contains("authenticationmethod")) // get authentication method claim value
                        saml2User.AuthenticatioMethod = sclaim.Value;
                    if (sclaim.Type.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(sclaim.Value, out parsedDate))
                            saml2User.AuthenticationInstant = parsedDate;
                    }
                    if (sclaim.Type.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        saml2User.LogoutNameIdentifier = sclaim.Value;
                    if (sclaim.Type.Contains("SessionIndex"))  // get session index claim value
                        saml2User.SessionIndex = Int32.Parse(sclaim.Value);
                }
            }

            return saml2User;
        }

        #endregion Saml2 authentication and saml2 claims

    }

}

