using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ThirdPartySignOn.Saml.Data
{

    /// <summary>
    /// Saml2UserInfoReduced represents reduced saml2 UserInfo
    /// </summary>
    public class SamlUserInfoReduced
    {

        #region properties

        public string NameIdentifier { get; set; }

        public string AuthenticatioMethod { get; set; }

        public DateTime AuthenticationInstant { get; set; }

        public string LogoutNameIdentifier { get; set; }

        public int SessionIndex { get; set; }

        public Dictionary<string, string> ClaimsDictionary { get; set; }

        #endregion properties

        #region ctors

        public SamlUserInfoReduced()
        {
            NameIdentifier = "Unauthorized Unknown";
            AuthenticatioMethod = string.Empty;
            AuthenticationInstant = DateTime.MinValue;
            LogoutNameIdentifier = string.Empty;
            SessionIndex = -1;
            ClaimsDictionary = new Dictionary<string, string>();
        }

        public SamlUserInfoReduced(AuthenticationStateProvider authStateProvider) : this()
        {
            // var authState = authStateProvider.GetAuthenticationStateAsync()).GetAwaiter().GetResult();
            var authState = Task.Run<AuthenticationState>(async () => await authStateProvider.GetAuthenticationStateAsync()).GetAwaiter().GetResult();

            var user = authState.User;
            
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                if (ClaimsDictionary == null)
                    ClaimsDictionary = new Dictionary<string, string>();
                else
                    ClaimsDictionary.Clear();

                foreach (var claim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    if (claim == null)
                        continue;

                    if (ClaimsDictionary.ContainsKey(claim.Type))
                        ClaimsDictionary[claim.Type] = claim.Value; // set claim value to existing claim type
                    else
                        ClaimsDictionary.Add(claim.Type, claim.Value); // add claim type / value to dictionary

                    if (claim.Type.Contains("nameidentifier")) // get name identifier claim value
                        NameIdentifier = claim.Value;
                    if (claim.Type.Contains("authenticationmethod")) // get authentication method claim value
                        AuthenticatioMethod = claim.Value;
                    if (claim.Type.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(claim.Value, out parsedDate))
                            AuthenticationInstant = parsedDate;
                    }
                    if (claim.Type.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        LogoutNameIdentifier = claim.Value;
                    if (claim.Type.Contains("SessionIndex"))  // get session index claim value
                        SessionIndex = Int32.Parse(claim.Value);
                }
            }          
        }


        public SamlUserInfoReduced(List<Claim> claimsList) : this()
        {            
            if (claimsList != null && claimsList.Count > 0)
            {
                if (ClaimsDictionary == null)
                    ClaimsDictionary = new Dictionary<string, string>();
                else
                    ClaimsDictionary.Clear();

                foreach (Claim claim in claimsList)
                {
                    if (ClaimsDictionary.ContainsKey(claim.Type))
                        ClaimsDictionary[claim.Type] = claim.Value; // set claim value to existing claim type
                    else
                        ClaimsDictionary.Add(claim.Type, claim.Value); // add claim type / value to dictionary

                    if (claim.Type.Contains("nameidentifier")) // get name identifier claim value
                        NameIdentifier = claim.Value;
                    if (claim.Type.Contains("authenticationmethod")) // get authentication method claim value
                        AuthenticatioMethod = claim.Value;
                    if (claim.Type.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(claim.Value, out parsedDate))
                            AuthenticationInstant = parsedDate;
                    }
                    if (claim.Type.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        LogoutNameIdentifier = claim.Value;
                    if (claim.Type.Contains("SessionIndex"))  // get session index claim value
                        SessionIndex = Int32.Parse(claim.Value);
                }
            }
        }

        #endregion ctors

    }

}
