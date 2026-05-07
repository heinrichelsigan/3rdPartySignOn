using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using ThirdPartySignOn.Saml.Data;
using ThirdPartySignOn.Saml.Services;

namespace ThirdPartySignOn.Saml
{
    public class Program
    {

        public const string AppName = "SSO.Saml";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            SamlIdentConfig? saml2 = new SamlIdentConfig("Saml2");
            string application_name = Saml2SettingsKeyReader.ApplicationName;
            string cookie_name = Saml2SettingsKeyReader.GetKeySetting("Saml2CookieName");
            string domain_name = Saml2SettingsKeyReader.GetKeySetting("DomainName");
            string saml2_entityId = (saml2 != null && !string.IsNullOrEmpty(saml2.EntityId)) ? 
                saml2.EntityId : Saml2SettingsKeyReader.GetKeySetting("Saml2:EntityId");
            string saml2_identityProvider_EntityId = (saml2 != null && !string.IsNullOrEmpty(saml2.EntityId)) ? 
                saml2.IdentityProvider.EntityId : Saml2SettingsKeyReader.GetKeySetting("Saml2:IdentityProvider:EntityId");
            string saml2_metadataLocation = (saml2 != null && !string.IsNullOrEmpty(saml2.IdentityProvider.MetadataLocation)) ? 
                saml2.IdentityProvider.MetadataLocation : Saml2SettingsKeyReader.GetKeySetting("Saml2:IdentityProvider:MetadataLocation");            
            string saml2_logoutLocation = (saml2 != null) ?
                saml2.LogoutLocation : Saml2SettingsKeyReader.GetKeySetting("Saml2:LogoutLocatíon");
            string saml2_identityProvider_logoutUrl = (!string.IsNullOrEmpty(saml2_logoutLocation)) ?
                saml2_logoutLocation : Saml2SettingsKeyReader.GetKeySetting("Saml2:IdentityProvider:LogoutUrl");

            if (string.IsNullOrEmpty(saml2_identityProvider_logoutUrl)) 
            {
                saml2_identityProvider_logoutUrl = "https://stubidp.sustainsys.com/Logout";
            }

            UriCreationOptions uriOpts = new UriCreationOptions() { DangerousDisablePathAndQueryCanonicalization = true };

            // EnablerLog.LogOriginMsg(AppName, "program started!");

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
                options.SPOptions.EntityId = new EntityId(saml2_entityId); // "http://myrandomapplication/samltesting"
                options.IdentityProviders.Add(
                    new IdentityProvider(
                        new EntityId(
                            saml2_identityProvider_EntityId), // "https://stubidp.sustainsys.com/Metadata"
                            options.SPOptions)
                    {
                        LoadMetadata = true,
                        MetadataLocation = saml2_metadataLocation, // "https://stubidp.sustainsys.com/Metadata"
                        AllowUnsolicitedAuthnResponse = true,
                        SingleLogoutServiceResponseUrl = new Uri(saml2_identityProvider_logoutUrl, uriOpts), 
                        // SingleLogoutServiceUrl = new Uri(saml2_identityProvider_logoutUrl, uriOpts),                         
                        SingleLogoutServiceBinding = Saml2BindingType.HttpRedirect                        
                    });
            })
            .AddCookie();
            
       
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<HttpContextAccessor>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<HttpClient>();                      

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
                options.DefaultPolicy = options.DefaultPolicy;
            });


            // builder.Services.AddMvc();
            // builder.Services.AddMvcCore();
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<AuthenticateStub>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            SamlLog.LogStatic(AppName, "program starting...");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SamlLog.LogStatic(AppName, "app.UseDeveloperExceptionPage()");
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                SamlLog.LogStatic(AppName, "app.UseExceptionHandler(\"/Error\")");
            }
           
            app.UseStaticFiles();
            app.UseRouting();
            SamlLog.LogStatic(AppName, "app.UseRouting()");

            if (saml2_entityId.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                app.UseHttpsRedirection();
                SamlLog.LogStatic(AppName, "app.UseHttpsRedirection()");
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            SamlLog.LogStatic(AppName, "app.UseAuthorization().UseAuthorization()");


            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            SamlLog.LogStatic(AppName, "app.MapBlazorHub().MapFallbackToPage(\"/_Host\")");

            app.Run();
        }
    }
}
