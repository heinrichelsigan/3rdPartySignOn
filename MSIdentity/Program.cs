using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using ThirdPartySignOn.MSIdentity;
using ThirdPartySignOn.MSIdentity.Data;


namespace ThirdPartySignOn.MSIdentity
{

    /// <summary>
    /// Authenticate with
    /// User: guest@heinrihelsiganlive355.onmicrosoft.com
    /// write for pass an email to heinrich.elsigan@gmail.com or he@area23.at
    /// </summary>
    public class Program
    {
        public static readonly string appName = "3rdPartySignOn.MSIdentity";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
            builder.Services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();            

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<HttpContextAccessor>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<HttpClient>();

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
            });

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSingleton<AuthenticateStub>();

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
            ThirdPartySignOnLog.LogStatic(appName, "app.UseRouting()");

            string urlapp = SettingsKeyReader.AzureRedirectUrl;
            if (!string.IsNullOrEmpty(urlapp) && urlapp.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                app.UseHttpsRedirection();
                ThirdPartySignOnLog.LogStatic(appName, "app.UseHttpsRedirection()");
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            ThirdPartySignOnLog.LogStatic(appName, "app.UseAuthorization().UseAuthorization()");


            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            ThirdPartySignOnLog.LogStatic(appName, "app.UseAuthorization().UseAuthorization()");


            app.Run();
        }
    }
}
