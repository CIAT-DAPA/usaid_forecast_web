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

namespace CIAT.DAPA.USAID.Forecast.Web
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
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add custom settings from configuration file
            services.Configure<Settings>(options =>
            {
                options.api_fs = Configuration.GetSection("API_Forecast:api_fs").Value;
                options.api_fs_geographic = Configuration.GetSection("API_Forecast:api_fs_geographic").Value;
                options.api_fs_agronomic = Configuration.GetSection("API_Forecast:api_fs_agronomic").Value;
                options.api_fs_forecast_climate = Configuration.GetSection("API_Forecast:api_fs_forecast_climate").Value;
                options.api_fs_forecast_yield = Configuration.GetSection("API_Forecast:api_fs_forecast_yield").Value;
                options.api_fs_historical = Configuration.GetSection("API_Forecast:api_fs_historical").Value;
                options.api_fs_historical_yield_years = Configuration.GetSection("API_Forecast:api_fs_historical_yield_years").Value;
                options.api_fs_historical_yield = Configuration.GetSection("API_Forecast:api_fs_historical_yield").Value;
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
