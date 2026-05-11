using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using SamlAuthGateway.Data;
using SamlAuthGateway.Services;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using System.IO;
using System.Reflection;

namespace SamlAuthGateway
{

    /// <summary>
    /// Console Program, that launches Kestrel Blazor App
    /// </summary>
    public class Program
    {
        public static string AppName = Assembly.GetExecutingAssembly().GetName().Name ?? "SamlAuthGateway";

        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);
            SamlIdentConfig? saml2 = new SamlIdentConfig("Saml2");
            // appName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath);           
            string appName = SettingsKeyReader.ApplName;
            var procPath = (string.IsNullOrEmpty(Environment.ProcessPath) ? AppName :
                            Path.GetFileNameWithoutExtension(Environment.ProcessPath));
            AppName = (!string.IsNullOrEmpty(appName)) ? appName : procPath;
            string cookie_name = SettingsKeyReader.SamlCookie;
            string domain_name = SettingsKeyReader.HostDomain;            
            string saml2_logoutUrl = (!string.IsNullOrEmpty(saml2.LogoutLocation)) ?
                saml2.LogoutLocation : saml2.IdentityProvider.LogoutUrl;
            if (string.IsNullOrEmpty(saml2_logoutUrl)) 
                saml2_logoutUrl = "https://stubidp.sustainsys.com/Logout";

            UriCreationOptions uriOpts = new UriCreationOptions() { DangerousDisablePathAndQueryCanonicalization = true };


            AuthenticationOptions authOptions = new AuthenticationOptions()
            {
                DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme,                
                DefaultChallengeScheme = "Saml2"
            };

            builder.Services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = "Saml2";
            })
            .AddSaml2(options =>
            {
                options.SPOptions.EntityId = new EntityId(saml2.EntityId); // "http://myrandomapplication/samltesting"
                options.IdentityProviders.Add(
                    new IdentityProvider(
                        new EntityId(
                            saml2.IdentityProvider.EntityId), // "https://stubidp.sustainsys.com/Metadata"
                            options.SPOptions)
                    {
                        LoadMetadata = true,                        
                        MetadataLocation = saml2.IdentityProvider.MetadataLocation, // "https://stubidp.sustainsys.com/Metadata"
                        AllowUnsolicitedAuthnResponse = true, 
                       
                        SingleLogoutServiceResponseUrl = new Uri(saml2_logoutUrl, uriOpts), 
                        // SingleLogoutServiceUrl = new Uri(saml2_identityProvider_logoutUrl, uriOpts),                         
                        SingleLogoutServiceBinding = Saml2BindingType.HttpPost                        
                    });
            })
            .AddCookie();
            
       
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<HttpContextAccessor>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<ICookie, Cookie>();                        

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
                options.DefaultPolicy = options.DefaultPolicy;
            });

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<AuthStub>();
            builder.Services.AddSingleton<SettingsKeyReader>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            SamlLog.LogOriginMsg(Program.AppName,
                $"program starting, config:\n{JsonConvert.SerializeObject(saml2)}");


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SamlLog.LogOriginMsg(AppName, "app.UseDeveloperExceptionPage()");
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                SamlLog.LogOriginMsg(AppName, "app.UseExceptionHandler(\"/Error\"); app.UseHsts();");
            }
           
            app.UseStaticFiles();
            app.UseRouting();
            SamlLog.LogOriginMsg(AppName, "app.UseRouting()");

            if (saml2.EntityId.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                app.UseHttpsRedirection();
                SamlLog.LogOriginMsg(AppName, "app.UseHttpsRedirection()");
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            SamlLog.LogOriginMsg(AppName, "app.UseAuthorization(); app.UseAuthorization(); app.UseCookiePolicy();");

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            SamlLog.LogOriginMsg(AppName, "app.MapBlazorHub(); app.MapFallbackToPage(\"/_Host\")");

            app.Run();
        }
    }
}
