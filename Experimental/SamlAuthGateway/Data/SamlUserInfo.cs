using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SamlAuthGateway.Data
{

    /// <summary>
    /// Saml2UserInfoReduced represents reduced saml2 UserInfo
    /// </summary>
    public class SamlUserInfo
    {

        #region properties

        public System.Security.Claims.ClaimsPrincipal? Principal { get; internal set; }

        public string LoginName {  get => (Principal == null  || string.IsNullOrEmpty(Principal?.Identity?.Name)) ? 
                NameIdentifier : Principal?.Identity?.Name; } 

        public string NameIdentifier { get; internal set; }

        public string AuthenticatioMethod { get; internal set; }

        public DateTime AuthenticationInstant { get; internal set; }

        public string LogoutNameIdentifier { get; internal set; }

        public int SessionIndex { get; internal set; }

        public Dictionary<string, string> ClaimsDictionary { get; internal set; }

        #endregion properties

        #region ctors

        public SamlUserInfo()
        {
            NameIdentifier = "";
            AuthenticatioMethod = "";
            AuthenticationInstant = DateTime.MinValue;
            LogoutNameIdentifier = "";
            SessionIndex = -1;
            ClaimsDictionary = new Dictionary<string, string>();
        }

        public SamlUserInfo(AuthenticationStateProvider authStateProvider) : this()
        {
            var authState = Task.Run<AuthenticationState>(async () => await authStateProvider.GetAuthenticationStateAsync()).GetAwaiter().GetResult();
            SamlUserInfo usr = SamlUserInfo.GetSaml2UserFromAuthState(authState);
            if (usr != null)
            {
                this.AuthenticatioMethod = usr.AuthenticatioMethod;
                this.Principal = usr.Principal;
                this.ClaimsDictionary = usr.ClaimsDictionary;
                this.NameIdentifier = usr.NameIdentifier;
                this.AuthenticationInstant = usr.AuthenticationInstant;
                this.LogoutNameIdentifier = usr.LogoutNameIdentifier;
                this.SessionIndex = usr.SessionIndex;
            }
        }

        public SamlUserInfo(AuthenticationState? authState) : this()
        {
            SamlUserInfo usr = SamlUserInfo.GetSaml2UserFromAuthState(authState);
            if (usr != null)
            {
                this.AuthenticatioMethod = usr.AuthenticatioMethod;
                this.Principal = usr.Principal;
                this.ClaimsDictionary = usr.ClaimsDictionary;
                this.NameIdentifier = usr.NameIdentifier;
                this.AuthenticationInstant = usr.AuthenticationInstant;
                this.LogoutNameIdentifier = usr.LogoutNameIdentifier;
                this.SessionIndex = usr.SessionIndex;
            }
        }
     

        #endregion ctors

        public static SamlUserInfo GetSaml2UserFromAuthState(AuthenticationState? authState)
        {
            SamlUserInfo usr = new SamlUserInfo();
            usr.Principal = authState?.User;

            if (usr.Principal != null && usr.Principal.Identity is not null && usr.Principal.Identity.IsAuthenticated)
            {
                if (usr.ClaimsDictionary == null)
                    usr.ClaimsDictionary = new Dictionary<string, string>();
                else
                    usr.ClaimsDictionary.Clear();

                foreach (var sclaim in ((ClaimsIdentity)usr.Principal.Identity).Claims)
                {
                    if (sclaim == null)
                        continue;

                    if (usr.ClaimsDictionary.ContainsKey(sclaim.Type))
                        usr.ClaimsDictionary[sclaim.Type] = sclaim.Value;
                    else
                        usr.ClaimsDictionary.Add(sclaim.Type, sclaim.Value); // add claim type / value to dictionary

                    if (sclaim.Type.Contains("nameidentifier")) // get name identifier claim value
                        usr.NameIdentifier = sclaim.Value;
                    if (sclaim.Type.Contains("authenticationmethod")) // get authentication method claim value
                        usr.AuthenticatioMethod = sclaim.Value;
                    if (sclaim.Type.Contains("authenticationinstant")) // get authentication instant claim value
                    {
                        DateTime parsedDate = DateTime.Now.AddDays(-1);
                        if (DateTime.TryParse(sclaim.Value, out parsedDate))
                            usr.AuthenticationInstant = parsedDate;
                    }
                    if (sclaim.Type.Contains("LogoutNameIdentifier")) // get logout name identifier claim value
                        usr.LogoutNameIdentifier = sclaim.Value;
                    if (sclaim.Type.Contains("SessionIndex"))  // get session index claim value
                        usr.SessionIndex = Int32.Parse(sclaim.Value);
                }
            }
            
            return usr;
        }

    }

}
