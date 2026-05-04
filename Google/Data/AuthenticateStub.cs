using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace ThirdPartySignOn.Google.Data
{

    /// <summary>
    /// AuthenticateStub is service to get username and claims
    /// </summary>
    public class AuthenticateStub
    {
        public static List<System.Security.Claims.Claim> ClaimsList { get; private set; }
        public static List<string> LoginNames { get; private set; }
        public static string UserName { get; private set; }
        public static string RealName { get; private set; }

        static AuthenticateStub()
        {
            RealName = "";
            UserName = "";
            LoginNames = new List<string>();
            ClaimsList = new List<System.Security.Claims.Claim>();
        }

        /// <summary>
        /// GetLoginNameIdentifier gets the username from the name identifier claim. This method is intended for testing purposes only and should not be used in production.
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{string}">Task containing username</see></returns>
        public async Task<string[]> GetLoginNameIdentifier(AuthenticationStateProvider authStateProvider)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (LoginNames == null || LoginNames.Count == 0) 
                LoginNames = new List<string>();
            
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                UserName = user.Identity.Name ?? "";
                if (!string.IsNullOrEmpty(UserName) && !LoginNames.Contains(UserName))
                    LoginNames.Add(UserName);
                // string displayName = user.GetDisplayName() ?? "[no display naame]";
                // if (!LoginNames.Contains(displayName))
                // LoginNames.Add(displayName);

                if (ClaimsList == null || ClaimsList.Count == 0)
                    ClaimsList = new List<Claim>();
                else
                    ClaimsList.Clear();

                foreach (var claim in (((ClaimsIdentity)user.Identity).Claims).ToList<System.Security.Claims.Claim>())
                {
                    if (claim != null)
                    {
                        ClaimsList.Add(claim);
                        if (claim.Type != null && claim.Type.EndsWith("username") && !string.IsNullOrEmpty(claim.Value))
                        {
                            UserName = claim.Value;
                            if (!LoginNames.Contains(claim.Value))
                                LoginNames.Insert(0, claim.Value);
                        }
                        if (claim.Type != null && claim.Type.Equals("name", StringComparison.OrdinalIgnoreCase))
                        {
                            RealName = claim.Value;
                        }
                    }
                }               
            }
            
            return LoginNames.ToArray(); 
        }

    }
}
