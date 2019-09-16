using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories;

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

        public async Task<IActionResult> Index()
        {
            try
            {
                // Load the urls of the web api's
                loadAPIs();
                // Set the parameters
                ViewBag.Section = SectionSite.Climate;

                // Searching the weather station, if the parameters don't come, it will redirect a default weather station
                RepositoryWeatherStations rWS = new RepositoryWeatherStations(Root);
                var ws = await rWS.ListAsync();
                return View(ws);
            }
            catch(Exception ex)
            {
                return View("Error");
            }
        }

        public IActionResult Glosario()
        {
            // Load the urls of the web api's
            loadAPIs();
            ViewBag.Section = SectionSite.Glossary;
            return View();
        }

        public IActionResult AcercaDe()
        {
            // Load the urls of the web api's
            loadAPIs();
            ViewBag.Section = SectionSite.About;
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
