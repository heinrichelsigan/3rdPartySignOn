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

                foreach (var sclaim in ((ClaimsIdentity)user.Identity).Claims)
                {
                    if (sclaim == null)
                        continue;

                    SamlClaimReduced rClaim = new SamlClaimReduced(sclaim); // get reduced claim
                    ClaimsDictionary.Add(rClaim.ClaimType, rClaim.ClaimValue); // add claim type / value to dictionary

                    if (rClaim.ClaimType.Contains("nameidentifier")) // get name identifier claim value
                        NameIdentifier = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("authenticationmethod")) // get authentication method claim value
                        AuthenticatioMethod = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(rClaim.ClaimValue, out parsedDate))
                            AuthenticationInstant = parsedDate;
                    }
                    if (rClaim.ClaimType.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        LogoutNameIdentifier = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("SessionIndex"))  // get session index claim value
                        SessionIndex = Int32.Parse(rClaim.ClaimValue);

                }
            }          
        }


        public SamlUserInfoReduced(List<SamlClaimReduced> claimsList) : this()
        {            
            if (claimsList != null && claimsList.Count > 0)
            {
                if (ClaimsDictionary == null)
                    ClaimsDictionary = new Dictionary<string, string>();
                else
                    ClaimsDictionary.Clear();

                foreach (SamlClaimReduced sclaim in claimsList)
                {
                    SamlClaimReduced rClaim = new SamlClaimReduced(sclaim ?? SamlClaimReduced.EmptyClaim); // get reduced claim
                    ClaimsDictionary.Add(rClaim.ClaimType, rClaim.ClaimValue); // add claim type / value to dictionary

                    if (rClaim.ClaimType.Contains("nameidentifier")) // get name identifier claim value
                        NameIdentifier = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("authenticationmethod")) // get authentication method claim value
                        AuthenticatioMethod = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(rClaim.ClaimValue, out parsedDate))
                            AuthenticationInstant = parsedDate;
                    }
                    if (rClaim.ClaimType.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        LogoutNameIdentifier = rClaim.ClaimValue;
                    if (rClaim.ClaimType.Contains("SessionIndex"))  // get session index claim value
                        SessionIndex = Int32.Parse(rClaim.ClaimValue);
                }
            }
        }

        #endregion ctors

    }

}
