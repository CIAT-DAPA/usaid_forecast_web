﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Services;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Identity;
using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Models;

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
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        private ConfigContext confContext { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add custom settings from configuration file
            services.Configure<Settings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("ForecastConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("ForecastConnection:Database").Value;
                options.LogPath = Configuration.GetSection("Data:Log").Value;
                options.ImportPath = Configuration.GetSection("Data:Imports").Value;
                options.ConfigurationPath = Configuration.GetSection("Data:Configuration").Value;
            });

            // Register the configuration settings
            confContext = new Models.Tools.ConfigContext(Configuration.GetSection("ForecastConnection:ConnectionString").Value, Configuration.GetSection("ForecastConnection:Database").Value, Configuration.GetSection("Security:AdminUser").Value, Configuration.GetSection("Security:AdminPassword").Value);

            // Register identity framework services and also Mongo storage. 
            services.AddIdentityWithMongoStores(Configuration.GetSection("ForecastConnection:ConnectionString").Value)
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
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(5);
                options.Cookies.ApplicationCookie.LoginPath = "/Account/LogIn";
                options.Cookies.ApplicationCookie.LogoutPath = "/Account/LogOff";
                // User settings
                options.User.RequireUniqueEmail = true;
                // Signin settings
                options.SignIn.RequireConfirmedEmail = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();

            // Add framework services
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
            // Setting the users and roles for the app
            Task.Run(async () =>
            {
                await confContext.CreateRolesAndUserAsync();
            });
        }
    }
}
