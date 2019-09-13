using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using Microsoft.AspNetCore.Hosting;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class CultivoController : WebBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public CultivoController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment): base(settings, hostingEnvironment)
        {
        }

        // GET: /Cultivo/state/municipality/station/crop
        [Route("/[controller]/{state?}/{municipality?}/{station?}/{crop?}")]
        public async Task<IActionResult> Index(string state, string municipality, string station, string crop)
        {
            try
            {
                // Load the urls of the web api's
                loadAPIs();
                // Load the dates of the forecast
                loadMonthsCrop();
                // Set the parameters                
                ViewBag.s = state ?? string.Empty;
                ViewBag.m = municipality ?? string.Empty;
                ViewBag.w = station ?? string.Empty;
                ViewBag.c = crop ?? string.Empty;
                ViewBag.Section = SectionSite.Crop;
                return View();
            }
            catch(Exception ex)
            {
                return View("Error");
            }            
        }
    }
}
