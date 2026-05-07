using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Security.Cryptography.X509Certificates;
using ThirdPartySignOn.Google.Data;


namespace ThirdPartySignOn.Google
{

    /// <summary>
    /// Authenticate with
    /// User: guest@heinrihelsiganlive355.onmicrosoft.com
    /// write for pass an email to heinrich.elsigan@gmail.com or he@area23.at
    /// </summary>
    public class Program
    {
        public static readonly string appName = "SSO.Google";        

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string smime = GoogleSettingsKeyReader.GetKeySetting("Authentication:Google:SMime");
            string clientSecret = CryptExtensions.FromBase64(smime).Replace("\n", "").Replace("\r", "");


            // Add services to the container.
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            builder.Services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = GoogleSettingsKeyReader.GetKeySetting("Authentication:Google:ClientId");
                options.ClientSecret = clientSecret;
                options.TokenEndpoint = GoogleSettingsKeyReader.GetKeySetting("Authentication:Google:TokenUri");
                options.AuthorizationEndpoint = GoogleSettingsKeyReader.GetKeySetting("Authentication:Google:AuthUri");
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            builder.Services.AddHttpClient("AuthUri", client =>
            {
                client.BaseAddress = new Uri(GoogleSettingsKeyReader.GetKeySetting("Authentication:Google:AuthUri"));
            });
            //builder.Services
            //    .AddScoped<IGoogleConnectService, GoogleConnectService>()
            //    .AddScoped<GoogleConnectService>()
            //    .AddScoped<AuthenticationStateProvider>(provider =>
            //        provider.GetRequiredService<GoogleConnectService>());
            //builder.Services.AddAuthentication(options => 
            //    {
            //        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters.NameClaimType = "sub";
            //        options.TokenValidationParameters.RoleClaimType = "role";
            //    })
            //    .AddGoogle(options =>
            //    {
            //        IConfigurationSection googleAuthNSection =
            //            builder.Configuration.GetSection("Authentication:Google");
            //        options.ClientId = SettingsKeyReader.GetKeySetting("Authentication:Google:ClientId");
            //        options.ClientSecret = Crypt.FromBase64(SettingsKeyReader.GetKeySetting("Authentication:Google:ClientSMime"));
            //        options.TokenEndpoint = SettingsKeyReader.GetKeySetting("Authentication:Google:TokenUri");
            //        options.AuthorizationEndpoint = SettingsKeyReader.GetKeySetting("Authentication:Google:AuthUri");
            //        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    });

            //builder.Services.AddAuthentication().AddGoogle(googleOptions =>    
            //{                
            //             googleOptions.ClientId = SettingsKeyReader.GetKeySetting("Authentication:Google:ClientId");
            //             googleOptions.ClientSecret = Crypt.FromBase64(SettingsKeyReader.GetKeySetting("Authentication:Google:SMime"));
            //             googleOptions.TokenEndpoint = SettingsKeyReader.GetKeySetting("Authentication:Google:TokenUri");
            //             googleOptions.AuthorizationEndpoint = SettingsKeyReader.GetKeySetting("Authentication:Google:AuthUri");
            //             // googleOptions.AddCertificateClient<X509Certificate>()
            //         });    
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<HttpContextAccessor>();
            // builder.Services.AddHttpClient()
            builder.Services.AddScoped<HttpClient>();

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.GetPolicy("Google");
            });

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();
            builder.Services.AddSingleton<AuthenticateStub>();

            builder.Services.AddSingleton<GoogleConnectService>();

            var app = builder.Build();
            app.UseForwardedHeaders();
            // app.UsePathBase("/pages");


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 
            else 
            { 
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            GoogleLog.LogStatic(appName, "app.UseRouting()");

            string urlapp = GoogleSettingsKeyReader.RedirectUrl;
            if (!string.IsNullOrEmpty(urlapp) && urlapp.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                app.UseHttpsRedirection();
                GoogleLog.LogStatic(appName, "app.UseHttpsRedirection()");
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            GoogleLog.LogStatic(appName, "app.UseAuthorization().UseAuthorization()");


            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            GoogleLog.LogStatic(appName, "app.MapFallbackToPage(\"/_Host\")");


            app.Run();
        }
    }
}
