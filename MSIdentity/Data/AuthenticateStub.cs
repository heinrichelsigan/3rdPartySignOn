using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace ThirdPartySignOn.MSIdentity.Data
{
    public class AuthenticateStub
    {
        public List<System.Security.Claims.Claim> ClaimsList { get; private set; }
        public string UserName { get; private set; }

        public AuthenticateStub()
        {
            ClaimsList = new List<System.Security.Claims.Claim>();
            UserName = "";
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
            List<string> loginNameList = new List<string>();

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                UserName = user.Identity.Name ?? "";                
                loginNameList.Add(UserName);
                loginNameList.Add(user.GetDisplayName() ?? "[no display naame]");

                ClaimsList = (((ClaimsIdentity)user.Identity).Claims).ToList<System.Security.Claims.Claim>();
                foreach (var claim in ClaimsList)
                {
                    
                    if (claim != null && claim.Type != null && claim.Type.Contains("nameidentifier"))
                    {
                        loginNameList.Add(claim.Value);
                        break;
                    }
                }
                return loginNameList.ToArray(); ;
            }

            return [];
        }
    }
}
