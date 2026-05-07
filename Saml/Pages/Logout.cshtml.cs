// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using ThirdPartySignOn.Saml.Services;

namespace ThirdPartySignOn.Saml.Pages;

[IgnoreAntiforgeryToken]
public class LogoutModel : PageModel
{

    [BindProperty]
    public string? Action { get; set; }

    public IDictionary<string, string?>? Items { get; set; }

    public string LogoutDirectUrl { get; private set; } = "";

    public string UserName { get; private set; } = "";

    public bool IsAuthenticated { get; private set; } = false;

    public string? RequestId { get; set; }

    public string CurrentUrl { get; private set; } = "";

    public string RequestCurrentPath { get; private set; } = "";

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string[] Cookies { get; private set; } = Array.Empty<string>();

    

    internal string GetUserName()     
    {
        string userLoginName = "";
        if (User!.Identity!.IsAuthenticated)
        {
            userLoginName = User!.Identity!.Name;
            if (!string.IsNullOrEmpty(userLoginName))
            {
                IsAuthenticated = true;
                return userLoginName;
            }
        }
        HttpContext.Request.Query.TryGetValue("user", out var userValue);
        if (!string.IsNullOrEmpty(userValue.ToString()))
        {
            userLoginName = userValue.ToString();
            if (!string.IsNullOrEmpty(userLoginName))
            {
                IsAuthenticated = true;
                return userLoginName;
            }
        }
        HttpContext.Request.Query.TryGetValue("usercrypt", out var userCryptValue);
        if (!string.IsNullOrEmpty(userCryptValue.ToString()))
        {
            userLoginName = userCryptValue.ToString().DeCrypt();
            if (!string.IsNullOrEmpty(userLoginName))
            {
                IsAuthenticated = true;
                return userLoginName;
            }
        }

        return (!string.IsNullOrEmpty(userLoginName)) ? userLoginName : "anonymous";
    }

    internal string GetCurrentGWPath()
    {
        string currentGWPath = Saml2SettingsKeyReader.Saml2AuthGWPath;
        if (HttpContext.Request.Path.ToString().Contains("logout", StringComparison.OrdinalIgnoreCase))
        {
            int idx = HttpContext.Request.Path.ToString().IndexOf("logout", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
                currentGWPath = HttpContext.Request.Path.ToString().Substring(0, idx);
        }
        return currentGWPath;
    }

    internal string GetLogoutUrl(bool lastStep = false)
    {
        string logoutUrl = HttpContext.Request.GetDisplayUrl();
        string returnUrl = string.Format("{0}://{1}/{2}/",
                                ((HttpContext.Request.IsHttps) ? "https" : "http"),
                                HttpContext.Request.Host.ToString().Replace("/", ""),
                                HttpContext.Request.Path.ToString().Replace("/", ""));

        if (logoutUrl.Contains("logout", StringComparison.OrdinalIgnoreCase))
        {
            returnUrl = logoutUrl;
            if (logoutUrl.Contains("logout?user", StringComparison.InvariantCultureIgnoreCase))
            {
                string spattern = "?user";
                int idx = logoutUrl.IndexOf(spattern, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    returnUrl = logoutUrl.Substring(0, idx);
                    if (returnUrl.EndsWith("?"))
                        returnUrl = returnUrl.Replace("?", "");
                    returnUrl += "?logout=" + UserName;
                    return returnUrl;
                }
            }
            if (lastStep)
            {
                string spattern = "logout?";
                int idx = logoutUrl.IndexOf(spattern, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                    returnUrl = logoutUrl.Substring(0, idx);
            }
        }

        return returnUrl;
    }

    internal string GetRequestPath()
    {
        string reqPath = ((HttpContext.Request.IsHttps) ? "https" : "http") + "://" + HttpContext.Request.Host;
        reqPath = (reqPath.EndsWith("/")) ? reqPath : reqPath + "/" + HttpContext.Request.Path;
        return reqPath;
    }

    internal string[] GetCookies()
    {
        IRequestCookieCollection cookieCollection = HttpContext.Request.Cookies;
        int cookieCount = 0;
        List<string> cookyList = new List<string>(cookieCollection.Count);
        foreach (var cookie in cookieCollection)
        {
            cookyList.Add($"{cookie.Key}={cookie.Value}");
            cookieCount++;
        }

        return cookyList.ToArray();
    }

    internal string GetRequestAuthGWPath()
    {
        string reqAuthGWPath = ((HttpContext.Request.IsHttps) ? "https" : "http") + "://" + HttpContext.Request.Host;
        reqAuthGWPath = (reqAuthGWPath.EndsWith("/")) ? reqAuthGWPath : reqAuthGWPath + "/" + GetCurrentGWPath();
        return reqAuthGWPath;
    }

    internal void DeleteCookie(string cookieName = ".AspNetCore.Cookies")
    {
        try
        {
            var copts = new CookieOptions();
            copts.Expires = DateTimeOffset.UtcNow.AddDays(-1);
            copts.Secure = true;
            copts.Path = GetCurrentGWPath();
            copts.Domain = Saml2SettingsKeyReader.HostDomainName;
        
            HttpContext.Response.Cookies.Delete(cookieName, copts);
        }
        catch (Exception ex)
        {
            SamlLog.LogOriginMsgEx("LogoutModel.DeleteCookie", $"Error deleting cookie {cookieName}", ex);
        }
    }

    public async Task OnGet()
    {
        var authResult = await HttpContext.AuthenticateAsync();
        UserName = GetUserName();
        Cookies = GetCookies();
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;        
        RequestCurrentPath = GetCurrentGWPath();
        CurrentUrl = GetLogoutUrl(false);
        LogoutDirectUrl = !string.IsNullOrEmpty(Saml2SettingsKeyReader.LogoutUrl) ?
            Saml2SettingsKeyReader.LogoutUrl : GetLogoutUrl(true);
        Items = authResult?.Properties?.Items;       
        
        DeleteCookie(".AspNetCore.Cookies");

        //if (IsAuthenticated)
        //{
        //    double dseed = DateTime.UtcNow.Subtract(DateTime.Today).TotalSeconds;
        //    int seed = (int)dseed;
        //    Span<byte> hexBytes = new Span<byte>();
        //    Span<byte> srcBytes = new Span<byte>(Encoding.UTF8.GetBytes(dseed.ToString().EnCrypt()));
        //    if (Convert.TryToHexString(srcBytes, hexBytes, out int bytesWritten))
        //    {
        //        seed = Convert.ToInt32(hexBytes.ToString().Substring(0, 4), 16);
        //    }

        //    int waitSeconds = new Random(seed).Next(8, 48);
        //    MetaContentRefresh = $"{waitSeconds}; URL={LogoutDirectUrl}";

        bool lastLogoutReturnUrl = false;

        if (HttpContext.Request.Query.TryGetValue("user", out var jsUser) ||
            HttpContext.Request.Query.TryGetValue("usercrypt", out var jsUserCrypt)) {
            lastLogoutReturnUrl = false;
        }

        if (HttpContext.Request.Query.TryGetValue("logout", out var jsLogout) ||
            !string.IsNullOrEmpty(UserName))
        {            
            if ((UserName.Equals("anonymous", StringComparison.InvariantCultureIgnoreCase)) || 
                !string.IsNullOrEmpty(jsLogout))
                lastLogoutReturnUrl = true;

            string logReturnUrl = GetLogoutUrl(lastLogoutReturnUrl);
            
            if (!lastLogoutReturnUrl)
            {
                SendOrPostCallback callback = new((state) =>
                {
                    HttpContext.Response.Redirect(logReturnUrl);
                });
            }
            else
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Redirect("/");
            }                
            
            return;
        }

    }

    public async Task<IActionResult> OnPost()
    {
        switch (Action)
        {
            case "SignInStubIdp":
                {
                    AuthenticationProperties properties = new()
                    {
                        Items =
                        {
                            { "TestKey", "TestValue" }
                        },
                        RedirectUri = Request.PathBase
                    };
                    return Challenge(properties, "stubidp");
                }
            case "SignInIdSrv":
                {
                    return Challenge("idsrv");
                }
            case "SignOut":
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/");
            default:
                throw new NotImplementedException();
        }

    }

}