using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace CIAT.DAPA.USAID.Forecast.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env, IServiceProvider serviceProvider)
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

            // Add custom settings from configuration file
            services.Configure<Settings>(options =>
            {
                options.api_fs = Configuration.GetSection("API_Forecast:api_fs").Value;
                options.idCountry = Configuration.GetSection("API_Forecast:idCountry").Value;
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                // Add support for finding localized views, based on file name suffix, e.g. Index.fr.cshtml
                .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder);

            // Configure supported cultures and localization options
            services.Configure<RequestLocalizationOptions>(options =>
            {
                //string[] languages = Configuration.GetSection("Languages").Value.Split(",");
                var idCountry = Configuration.GetSection("API_Forecast:idCountry").Value;
                //string[] languages = new string[] { "en-US", "es-CO" };
                string[] languages;
                if (idCountry == "61e59d829d5d2486e18d2ea8")
                {
                    languages = new string[] { "en-US", "es-CO" };
                }
                else
                {
                    languages = new string[] { "en-US", "am-ET" };
                }

                CultureInfo[] supportedCultures = new CultureInfo[languages.Length];
                for (int i = 0; i < languages.Length; i++)
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
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
