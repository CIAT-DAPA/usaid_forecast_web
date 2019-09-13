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
    public class ExpertoController : WebBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public ExpertoController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, hostingEnvironment)
        {
        }

        // GET: /Experto/Index/
        public async Task<IActionResult> Index()
        {
            try
            {
                // Load the urls of the web api's
                loadAPIs();
                // Load the dates of the forecast
                loadMonthsClimate();
                ViewBag.Section = SectionSite.Expert;
                return View();
            }
            catch(Exception ex)
            {
                return View("Error");
            }
        }
    }
}
