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
                // Set the parameters
                ViewBag.Section = SectionSite.Climate;

                // Setting data
                SetWS();

                return View();
            }
            catch(Exception ex)
            {
                return View("Error");
            }
        }

        public IActionResult Glosario()
        {
                       
            ViewBag.Section = SectionSite.Glossary;

            // Setting data
            SetWS();

            return View();
        }

        public IActionResult AcercaDe()
        {
            ViewBag.Section = SectionSite.About;

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
            string url = System.Web.HttpUtility.UrlPathEncode(returnUrl); //"~/";
            /*url = url + string.Join(
                "/",
                returnUrl.Replace("~/","").Split("/").Select(s => HttpUtility.UrlEncode(s))
            );*/
            
            return Redirect(url);
        }
    }
}
