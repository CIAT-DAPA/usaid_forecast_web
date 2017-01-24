using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class ClimaController : WebBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public ClimaController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment): base(settings, hostingEnvironment)
        {
        }

        // GET: /Clima/Index/?municipio=
        public async Task<IActionResult> Index(string municipio)
        {
            // Section to get a default municipality if it doesn't exist
            var m = await apiForecast.getMunicipalitiesAsync();
            if (string.IsNullOrEmpty(municipio) || m.Where(p=>p.name.Equals(municipio)).Count() < 1)
            {
                municipio = m.FirstOrDefault().name;
                return RedirectToAction("Index", new { municipio = municipio });
            }
            // Load the urls of the web api's
            loadAPIs();
            // Load the dates of the forecast
            loadMonthsClimate();
            return View();
        }
    }
}
