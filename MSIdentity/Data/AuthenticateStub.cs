using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace ThirdPartySignOn.MSIdentity.Data
{

    /// <summary>
    /// AuthenticateStub is service to get username and claims
    /// </summary>
    public class AuthenticateStub
    {
        public static List<System.Security.Claims.Claim> ClaimsList { get; private set; }
        public Dictionary<string, string> DictNames { get; private set; }
        public static string UserName { get; private set; }

        static AuthenticateStub()
        {
            UserName = "";            
            ClaimsList = new List<System.Security.Claims.Claim>();
        }

        /// <summary>
        /// GetLoginNameIdentifier gets the username from the name identifier claim. This method is intended for testing purposes only and should not be used in production.
        /// </summary>
        /// <param name="authStateProvider"></param>
        /// <returns><see cref="Task{string}">Task containing username</see></returns>
        public async Task<Dictionary<string, string>> GetLoginIds(AuthenticationStateProvider authStateProvider)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;            

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                if (DictNames == null || DictNames.Count == 0)
                    DictNames = new Dictionary<string, string>();
                else
                    DictNames.Clear();

                UserName = user.Identity.Name ?? "";
                if (!string.IsNullOrEmpty(UserName))
                {
                    if (!DictNames.ContainsKey("User.Identity.Name"))
                        DictNames.Add("User.Identity.Name", UserName);
                    else
                        DictNames["User.Identity.Name"] = UserName;
                }
                string displayName = user.GetDisplayName() ?? "";
                if (!string.IsNullOrEmpty(displayName))
                {
                    if (!DictNames.ContainsKey("User.DisplayName"))
                        DictNames.Add("User.DisplayName", displayName);
                    else
                        DictNames["User.DisplayName"] = displayName;
                }

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
                            if (!DictNames.ContainsKey(claim.Type))
                                DictNames.Add(claim.Type, claim.Value);
                            else
                                DictNames[claim.Type] = claim.Value;
                        }
                        if (claim.Type != null && claim.Type.Equals("name", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!DictNames.ContainsKey(claim.Type))
                                DictNames.Add(claim.Type, claim.Value);
                            else
                                DictNames[claim.Type] = claim.Value.ToString();
                        }
                    }
                }               
            }

            return DictNames;
        }

    }
}
