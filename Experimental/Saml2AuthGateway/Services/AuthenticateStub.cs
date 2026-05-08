using Microsoft.AspNetCore.Components.Authorization;
using Saml2AuthGateway.Data;
using System.Security.Claims;

namespace Saml2AuthGateway.Services
{

    /// <summary>
    /// a stub to get username and claims
    /// </summary>
    public class AuthenticateStub
    {

        #region Saml2 authentication and saml2 claims

        public List<Saml2ClaimReduced> claimsList = new List<Saml2ClaimReduced>();
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
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                foreach (var claim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    Saml2ClaimReduced rClaim = new Saml2ClaimReduced(claim);
                    if (rClaim.ClaimType.Contains("nameidentifier"))
                    {
                        UserName = rClaim.ClaimValue;
                        break;
                    }
                }
                return UserName;
            }
            return "Not Authenticated";
        }

        /// <summary>
        /// GetAuthClaims
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{List{Saml2ClaimReduced}}"/></returns>
        public async Task<List<Saml2ClaimReduced>> GetAuthClaims(AuthenticationStateProvider authStateProvider)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                if (claimsList == null || claimsList.Count == 0)
                    claimsList = new List<Saml2ClaimReduced>();
                else
                    claimsList.Clear();

                foreach (var claim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    Saml2ClaimReduced rClaim = new Saml2ClaimReduced(claim);
                    if (rClaim.ClaimType.Contains("nameidentifier"))
                        UserName = rClaim.ClaimValue;

                    claimsList.Add(rClaim);
                }
                return claimsList;
            }
            else
            {
                return new List<Saml2ClaimReduced>();
            }
        }

        /// <summary>
        /// GetSaml2UserInfoReduced
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{Saml2UserInfoReduced}"/></returns>
        public async Task<Saml2UserInfoReduced> GetSaml2UserInfoReduced(AuthenticationStateProvider authStateProvider)
        {
            Saml2UserInfoReduced saml2User = new Saml2UserInfoReduced();
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

                    Saml2ClaimReduced rClaim = new Saml2ClaimReduced(sclaim); // get reduced claim
                    if (saml2User.ClaimsDictionary.ContainsKey(rClaim.ClaimType))
                        saml2User.ClaimsDictionary[rClaim.ClaimType] = rClaim.ClaimValue; // set claim value to existing claim type
                    else
                        saml2User.ClaimsDictionary.Add(rClaim.ClaimType, rClaim.ClaimValue); // add claim type / value to dictionary

                    if (rClaim.ClaimType.Contains("nameidentifier")) // get name identifier claim value
                        saml2User.NameIdentifier = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("authenticationmethod")) // get authentication method claim value
                        saml2User.AuthenticatioMethod = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(rClaim.ClaimValue, out parsedDate))
                            saml2User.AuthenticationInstant = parsedDate;
                    }
                    if (rClaim.ClaimType.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        saml2User.LogoutNameIdentifier = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("SessionIndex"))  // get session index claim value
                        saml2User.SessionIndex = Int32.Parse(rClaim.ClaimValue);
                }
            }

            return saml2User;
        }

        #endregion Saml2 authentication and saml2 claims

        #region ws proxy calls

        [Obsolete("Method transfered to Saml2SoapClient.GetFormsTicket(string userName, bool isEncrypted = false)", false)]
        public string GetFormsAuthenticationTicketForUser(string username)
        {
            return Saml2SoapClient.GetFormsTicket(username, false);
        }

        #endregion ws proxy calls
    }

}

