using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class HomeController : WebBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public HomeController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, hostingEnvironment)
        {
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Clima");
        }

        public IActionResult Glosario()
        {
            // Load the urls of the web api's
            loadAPIs();
            return View();
        }

        public IActionResult AcercaDe()
        {
            // Load the urls of the web api's
            loadAPIs();
            return View();
        }
    }
}
