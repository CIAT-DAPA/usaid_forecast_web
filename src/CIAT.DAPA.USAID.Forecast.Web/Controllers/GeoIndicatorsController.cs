using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class GeoIndicatorsController : WebBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public GeoIndicatorsController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, hostingEnvironment)
        {
        }

        // GET: /GeoIndicators/
        [Route("/[controller]/")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Set the parameters
                ViewBag.Section = SectionSite.Indicators;
                // Setting data
                SetWS();
            
                loadConfigurations();
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View("Error");
            }
        }



        private void loadConfigurations()
        {
            ViewBag.geoserver_url = Configurations.indicator_geoserver_url;
            ViewBag.geoserver_workspace = Configurations.indicator_geoserver_workspace;
        }
    }
}
