using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using Microsoft.AspNetCore.Hosting;

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

        // GET: /Cultivo/Index/?municipio=&cultivo=
        public async Task<IActionResult> Index(string municipio, string cultivo)
        {
            // Section to get a default municipality if it doesn't exist
            var m = await apiForecast.listMunicipalities();
            if (string.IsNullOrEmpty(municipio) || m.Where(p => p.name.Equals(municipio)).Count() < 1)
            {
                municipio = m.FirstOrDefault().name;
                return RedirectToAction("Index", new { municipio = municipio, cultivo = cultivo });
            }
            // Load the urls of the web api's
            loadAPIs();
            // Load the dates of the forecast
            loadMonthsCrop();
            return View();
        }
    }
}
