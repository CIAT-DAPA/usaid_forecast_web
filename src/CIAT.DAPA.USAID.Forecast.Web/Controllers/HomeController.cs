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
using System.Web;
using System.IO;

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

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Set the parameters
                ViewBag.Section = SectionSite.Climate;
                // Setting data
                SetWS();

                return View();
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [Route("[controller]/[action]")]
        public IActionResult Glosario()
        {

            ViewBag.Section = SectionSite.Glossary;
            ViewBag.words = new string[] {
                "bio_acu", "d_dry", "d_har", "eva", "conf_int", "prec", "prec_acu", "climate", "pro_his",
                "forecast", "sol_rad", "yield", "yield_pot", "t_max", "t_max_acu", "t_min", "t_min_acu"
            };
            // Setting data
            SetWS();

            return View();
        }

        [Route("/[controller]/[action]")]
        public IActionResult AcercaDe()
        {
            ViewBag.Section = SectionSite.About;
            ViewBag.words = new string[] {
                "project", "scenarios", "yield_rice", "yield_maize", "validation_maize"
            };
            // Setting data
            SetWS();

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
            string url = System.Web.HttpUtility.UrlPathEncode(returnUrl); 
            return Redirect(url);
        }
    }
}
