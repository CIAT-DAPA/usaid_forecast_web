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
        protected string idCountry { get; set; }
        private string path { get; set; }
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public HomeController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, hostingEnvironment)
        {
            path = hostingEnvironment.ContentRootPath + "\\Log\\";
            idCountry = settings.Value.idCountry;
        }

        [Route("/{countryId?}")]
        public async Task<IActionResult> Index(string countryId)
        {
            try
            {
                // Set the parameters
                ViewBag.Section = SectionSite.Climate;
                countryId = idCountry;
                // Setting data
                SetWS(countryId);

                return View();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(path + DateTime.Now.ToString("yyyyMMdd"), "3... " + ex.Message.ToString() + "\n" + ex.InnerException.ToString() + "\n" + ex.StackTrace.ToString() + "\n");
                return View("Error");
            }
        }

        [Route("[controller]/[action]/{countryId?}")]
        public IActionResult Glosario(string countryId)
        {

            ViewBag.Section = SectionSite.Glossary;
            ViewBag.words = new string[] {
                "bio_acu", "d_dry", "d_har", "eva", "conf_int", "prec", "prec_acu", "climate", "pro_his",
                "forecast", "sol_rad", "yield", "yield_pot", "t_max", "t_max_acu", "t_min", "t_min_acu"
            };
            // Setting data
            SetWS(countryId);

            return View();
        }

        [Route("/[controller]/[action]/{countryId?}")]
        public IActionResult AcercaDe(string countryId)
        {
            ViewBag.Section = SectionSite.About;
            ViewBag.words = new string[] {
                "project", "scenarios", "yield_rice", "yield_maize", "validation_maize"
            };
            // Setting data
            SetWS(countryId);

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
