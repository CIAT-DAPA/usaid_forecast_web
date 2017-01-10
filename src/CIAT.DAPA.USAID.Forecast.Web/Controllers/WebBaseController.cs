using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class WebBaseController : Controller
    {
        protected IHostingEnvironment hostingEnvironment { get; set; }
        /// <summary>
        /// Get or set the object to get information from forecast service
        /// </summary>
        protected WebAPIForecast apiForecast { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public WebBaseController(IOptions<Settings> settings, IHostingEnvironment environment) : base()
        {
            hostingEnvironment = environment;
            apiForecast = new WebAPIForecast()
            {
                root = settings.Value.api_fs,
                geographic = settings.Value.api_fs_geographic,
                agronomic = settings.Value.api_fs_agronomic,
                forecast = settings.Value.api_fs_forecast,
                historical = settings.Value.api_fs_historical
            };
        }




    }
}
