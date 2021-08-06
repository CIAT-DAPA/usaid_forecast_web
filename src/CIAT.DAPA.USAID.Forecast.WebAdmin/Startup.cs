using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Identity;
using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;


using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
                
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine(Configuration);
            // Add custom settings from configuration file
            services.Configure<Settings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("ForecastConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("ForecastConnection:Database").Value;
                options.LogPath = Configuration.GetSection("Data:Log").Value;
                options.ImportPath = Configuration.GetSection("Data:Imports").Value;
                options.ConfigurationPath = Configuration.GetSection("Data:Configuration").Value;
                options.Installed = bool.Parse(Configuration.GetSection("Installed").Value);
                options.NotifyAccount = Configuration.GetSection("Notification:Account").Value;
                options.NotifyPassword = Configuration.GetSection("Notification:Password").Value;
                options.NotifyPort = int.Parse(Configuration.GetSection("Notification:Port").Value);
                options.NotifyServer = Configuration.GetSection("Notification:Server").Value;
                options.NotifySsl = bool.Parse(Configuration.GetSection("Notification:Ssl").Value);
                options.Languages = Configuration.GetSection("Languages").Value.Split(",");
                options.Crops = Configuration.GetSection("Crops").Value.Split(",");
            });
                        
            // Register identity framework services and also Mongo storage. 
            services.Configure<PasswordHasherOptions>(options =>
                    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2
                );

            services.AddIdentityMongoDbProvider<User, Role>(identityOptions =>
            {
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireLowercase = true;
                identityOptions.Password.RequireUppercase = true;
                identityOptions.Password.RequireNonAlphanumeric = true;
                identityOptions.Password.RequireDigit = true;
                identityOptions.User.RequireUniqueEmail = true;                
                //identityOptions.SignIn.RequireConfirmedEmail = true;
                //identityOptions
            }, mongoIdentityOptions => {
                mongoIdentityOptions.ConnectionString = Configuration.GetSection("ForecastConnection:ConnectionString").Value;
            }).AddDefaultTokenProviders();            

            /*services.AddIdentityWithMongoStores(Configuration.GetSection("ForecastConnection:ConnectionString").Value)
                .AddDefaultTokenProviders();

            // Settings to manage user
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                // Cookie settings
                //options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(5);
                //options.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
                //options.Cookies.ApplicationCookie.LogoutPath = "/Account/LogOff";
                // User settings
                options.User.RequireUniqueEmail = true;
                // Signin settings
                options.SignIn.RequireConfirmedEmail = true;
            }); */

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();

            // Add framework services
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                // Add support for finding localized views, based on file name suffix, e.g. Index.fr.cshtml
                .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder);

            // Configure supported cultures and localization options
            services.Configure<RequestLocalizationOptions>(options =>
            {
                string[] languages = Configuration.GetSection("Languages").Value.Split(",");
                //string[] languages = new string[] { "en-US","es-CO" };

                CultureInfo[] supportedCultures = new CultureInfo[languages.Length];
                for (int i=0;i<languages.Length;i++)
                {
                    supportedCultures[i] = new CultureInfo(languages[i]);
                }

                // State what the default culture for your application is. This will be used if no specific culture
                // can be determined for a given request.
                options.DefaultRequestCulture = new RequestCulture(culture: languages[0], uiCulture: languages[0]);

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {          

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
