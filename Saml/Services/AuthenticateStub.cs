using Microsoft.AspNetCore.Components.Authorization;
using ThirdPartySignOn.Saml.Data;
using System.Security.Claims;

namespace ThirdPartySignOn.Saml.Services
{

    /// <summary>
    /// a stub to get username and claims
    /// </summary>
    public class AuthenticateStub
    {

        #region Saml authentication and saml claims

        public List<Claim> claimsList = new List<Claim>();
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
                foreach (Claim claim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    if (claim.Type.Contains("nameidentifier"))
                    {
                        UserName = claim.Value;
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
        /// <returns><see cref="Task{List{Claim}}"/></returns>
        public async Task<List<Claim>> GetAuthClaims(AuthenticationStateProvider authStateProvider)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                if (claimsList == null || claimsList.Count == 0)
                    claimsList = new List<Claim>();
                else
                    claimsList.Clear();

                foreach (Claim claim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    if (claim.Type.Contains("nameidentifier"))
                        UserName = claim.Value;

                    claimsList.Add(claim);
                }
                return claimsList;
            }
            else
            {
                return new List<Claim>();
            }
        }

        /// <summary>
        /// GetSaml2UserInfoReduced
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{Saml2UserInfoReduced}"/></returns>
        public async Task<SamlUserInfoReduced> GetSamlUserInfoReduced(AuthenticationStateProvider authStateProvider)
        {
            SamlUserInfoReduced samlUser = new SamlUserInfoReduced();
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                if (samlUser.ClaimsDictionary == null)
                    samlUser.ClaimsDictionary = new Dictionary<string, string>();
                else
                    samlUser.ClaimsDictionary.Clear();

                foreach (Claim claim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    if (claim == null)
                        continue;

                    if (samlUser.ClaimsDictionary.ContainsKey(claim.Type))
                        samlUser.ClaimsDictionary[claim.Type] = claim.Value; // set claim value to existing claim type
                    else
                        samlUser.ClaimsDictionary.Add(claim.Type, claim.Value); // add claim type / value to dictionary

                    if (claim.Type.Contains("nameidentifier")) // get name identifier claim value
                        samlUser.NameIdentifier = claim.Value;
                    if (claim.Type.Contains("authenticationmethod")) // get authentication method claim value
                        samlUser.AuthenticatioMethod = claim.Value;
                    if (claim.Type.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(claim.Value, out parsedDate))
                            samlUser.AuthenticationInstant = parsedDate;
                    }
                    if (claim.Type.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        samlUser.LogoutNameIdentifier = claim.Value;
                    if (claim.Type.Contains("SessionIndex"))  // get session index claim value
                        samlUser.SessionIndex = Int32.Parse(claim.Value);
                }
            }

            return samlUser;
        }

        #endregion Saml authentication and saml claims


    }

}

