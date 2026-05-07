using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ThirdPartySignOn.Saml.Data
{

    /// <summary>
    /// Saml2UserInfoReduced represents reduced saml2 UserInfo
    /// </summary>
    public class SamlUserInfo
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

        public SamlUserInfo()
        {
            NameIdentifier = "Unauthorized Unknown";
            AuthenticatioMethod = string.Empty;
            AuthenticationInstant = DateTime.MinValue;
            LogoutNameIdentifier = string.Empty;
            SessionIndex = -1;
            ClaimsDictionary = new Dictionary<string, string>();
        }

        public SamlUserInfo(AuthenticationStateProvider authStateProvider) : this()
        {
            // var authState = authStateProvider.GetAuthenticationStateAsync()).GetAwaiter().GetResult();
            var authState = Task.Run<AuthenticationState>(
                                async () => await authStateProvider.GetAuthenticationStateAsync()
                            ).GetAwaiter().GetResult();

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
                    
                    if (ClaimsDictionary.ContainsKey(sclaim.Type))
                        ClaimsDictionary[sclaim.Type] = sclaim.Value;
                    else
                        ClaimsDictionary.Add(sclaim.Type, sclaim.Value); // add claim type / value to dictionary

                    if (sclaim.Type.Contains("nameidentifier")) // get name identifier claim value
                        NameIdentifier = sclaim.Value;
                    if (sclaim.Type.Contains("authenticationmethod")) // get authentication method claim value
                        AuthenticatioMethod = sclaim.Value;
                    if (sclaim.Type.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(sclaim.Value, out parsedDate))
                            AuthenticationInstant = parsedDate;
                    }
                    if (sclaim.Type.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        LogoutNameIdentifier = sclaim.Value;
                    if (sclaim.Type.Contains("SessionIndex"))  // get session index claim value
                        SessionIndex = Int32.Parse(sclaim.Value);

                }
            }          
        }

        #endregion ctors


        public static SamlUserInfo GetSaml2UserInfo(AuthenticationStateProvider authStateProvider)
        {
            SamlUserInfo samlUserInfo = new SamlUserInfo(authStateProvider);
            return samlUserInfo;
        }

    }

}
