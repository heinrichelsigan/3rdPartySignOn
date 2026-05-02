using Microsoft.AspNetCore.Components.Authorization;
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

        public List<SamlClaimReduced> claimsList = new List<SamlClaimReduced>();
        public string UserName = "";




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
                    SamlClaimReduced rClaim = new SamlClaimReduced(claim);
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
        public async Task<List<SamlClaimReduced>> GetAuthClaims(AuthenticationStateProvider authStateProvider)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                claimsList.Clear();
                foreach (var claim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    SamlClaimReduced rClaim = new SamlClaimReduced(claim);
                    if (rClaim.ClaimType.Contains("nameidentifier"))
                        UserName = rClaim.ClaimValue;

                    claimsList.Add(rClaim);

                }
                return claimsList;
            }
            else
            {
                return new List<SamlClaimReduced>();
            }
        }

        /// <summary>
        /// GetSaml2UserInfoReduced
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{SamlUserInfoReduced}"/></returns>
        public async Task<SamlUserInfoReduced> GetSamlUserInfoReduced(AuthenticationStateProvider authStateProvider)
        {
            SamlUserInfoReduced saml2User = new SamlUserInfoReduced();
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

                    SamlClaimReduced rClaim = new SamlClaimReduced(sclaim); // get reduced claim
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


        [Obsolete("This method is a stub and should not be used in production. It is intended for testing purposes only.", true)]
        public async Task<string> Logout(AuthenticationStateProvider authStateProvider)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            string logoutMsg = "";
            bool logoutPerformed = false;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                try
                {
                    logoutMsg = $"{UserName} will be logged out.";                    
                    // var outH = await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    UserName = "";
                    logoutPerformed = true;
                }
                catch (Exception ex)
                {
                    logoutMsg = $"Logout failed: {ex.Message}";
                    UserName = "";
                    logoutPerformed = false;
                }
            }

            try
            {
                user = null;
                authState = null;
                UserName = "";
            }
            catch
            {
                logoutMsg += "\t.com Exxception, when setting authState = null";
            }

            if (string.IsNullOrEmpty(logoutMsg) && logoutPerformed)
            {
                logoutMsg = "Logout successful.";
            }

            return logoutMsg;
        }


        /// <summary>
        /// Authenticate the user and get the username from the name identifier claim. This method is intended for testing purposes only and should not be used in production.
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns>Auth status message</returns>
        [Obsolete("This method is a stub and should not be used in production. It is intended for testing purposes only.", true)]
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

        #endregion Saml2 authentication and saml2 claims

    }

}

