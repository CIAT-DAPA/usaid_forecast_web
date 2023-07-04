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
using System.Net;
using Microsoft.Net.Http.Headers;


namespace CIAT.DAPA.USAID.Forecast.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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



            var partnersSection = Configuration.GetSection("Partners");
            var partners = new List<Dictionary<string, string>>();
            foreach (var partnerSection in partnersSection.GetChildren())
            {
                var partner = new Dictionary<string, string>
                {
                    ["name"] = partnerSection["name"],
                    ["link"] = partnerSection["link"],
                    ["src"] = partnerSection["src"]
                };
                partners.Add(partner);
            }
            var mapOverlaysSection = Configuration.GetSection("MapOverLays");
            var mapOverlays = new List<Dictionary<string, string>>();
            foreach (var mapOverlaySection in mapOverlaysSection.GetChildren())
            {
                var mapOverlay = new Dictionary<string, string>
                {
                    ["name"] = mapOverlaySection["name"],
                    ["src"] = mapOverlaySection["src"]
                };
                mapOverlays.Add(mapOverlay);
            }






            


            // Add custom settings from configuration file
            services.Configure<Settings>(options =>
            {
                options.api_fs = Configuration.GetSection("API_Forecast:api_fs").Value;
                options.idCountry = Configuration.GetSection("API_Forecast:idCountry").Value;

                options.gTag = Configuration.GetSection("SiteConfig:g-tag").Value;
                options.countryName = Configuration.GetSection("SiteConfig:countryName").Value;

                options.modules_climate = bool.Parse(Configuration.GetSection("Modules:Climate").Value);
                options.modules_indicators = bool.Parse(Configuration.GetSection("Modules:Indicators").Value);
                options.modules_geo_indicators = bool.Parse(Configuration.GetSection("Modules:GeoIndicators").Value);
                options.modules_rice = bool.Parse(Configuration.GetSection("Modules:Rice").Value);
                options.modules_maize = bool.Parse(Configuration.GetSection("Modules:Maize").Value);
                options.modules_expert = bool.Parse(Configuration.GetSection("Modules:Expert").Value);
                options.modules_glossary = bool.Parse(Configuration.GetSection("Modules:Glossary").Value);
                options.modules_about = bool.Parse(Configuration.GetSection("Modules:About").Value);

                options.partners = partners;
                options.mapOverlays = mapOverlays;


        
                if (options.modules_indicators || options.modules_geo_indicators)
                {
                    options.indicators_path = Configuration.GetSection("Indicators:IndicatorsPath").Value;
                    options.indicator_geoserver_url = Configuration.GetSection("Indicators:GeoserverUrl").Value;
                    options.indicator_geoserver_workspace = Configuration.GetSection("Indicators:GeoserverWorkspace").Value;

                    options.indicator_geoserver_average = int.Parse(Configuration.GetSection("Indicators:GeoserverAverage").Value);
                    options.indicator_geoserver_cv = int.Parse(Configuration.GetSection("Indicators:GeoserverCV").Value);
                    string[] limit = Configuration.GetSection("Indicators:GeoserverTime").Value.ToString().Split("-");
                    options.indicator_geoserver_time = new int[] { int.Parse(limit[0]), int.Parse(limit[1]) };
                    options.indicator_NINO = int.Parse(Configuration.GetSection("Indicators:GeoserverNINO").Value);
                    options.indicator_NINA = int.Parse(Configuration.GetSection("Indicators:GeoserverNINA").Value);
                }
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddResponseCaching();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                // Add support for finding localized views, based on file name suffix, e.g. Index.fr.cshtml
                .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder);

            // Configure supported cultures and localization options
            services.Configure<RequestLocalizationOptions>(options =>
            {                
                string[] languages = Configuration.GetSection("Cultures:Enable").Value.Split(",");
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
            app.UseResponseCaching();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
