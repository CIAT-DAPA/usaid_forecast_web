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

        // GET: /Cultivo/Index/?municipio=&cultivo=
        public async Task<IActionResult> Index(string municipio, string cultivo)
        {
            try
            {
                bool redir = false;

                // Section to get a default crop if it doesn't exist
                var c = await apiForecast.getAgronomicAsync();
                var data_crop = string.IsNullOrEmpty(cultivo) ? c.FirstOrDefault() : c.SingleOrDefault(p => p.cp_name.ToLower().Equals(cultivo.ToLower()));
                redir = string.IsNullOrEmpty(cultivo);
                if (data_crop == null)
                {
                    data_crop = c.FirstOrDefault();
                    redir = true;
                }
                string cultivo_temp = data_crop.cp_name;

                // Section to get a default municipality if it doesn't exist                        
                string municipio_temp = string.Empty;
                var m = await apiForecast.getMunicipalitiesAsync();
                var f = await apiForecast.getForecastAsync();
                List<string> ws = new List<string>();
                // This cicle search the municipalities with forecast data for the crop
                foreach (var my in f.yield)
                    foreach (var mc in my.yield)
                        foreach (var dc in data_crop.cultivars)
                            if (dc.id.Equals(mc.cultivar))
                                ws.Add(my.weather_station);
                var m_tmp = m.Where(p => p.weather_stations.Where(p2 => ws.Contains(p2.id)).Count() > 0);
                if (string.IsNullOrEmpty(municipio) || m_tmp.Where(p => p.name.Equals(municipio)).Count() < 1)
                {
                    municipio_temp = m_tmp.FirstOrDefault().name;
                    redir = true;
                }
                if (redir)
                    return RedirectToAction("Index", new { municipio = municipio_temp, cultivo = cultivo_temp.ToLower() });
                // List of municipalities available for the crop
                string municipalities = string.Empty;
                foreach (var mu in m_tmp)
                    municipalities += mu.id + ",";
                ViewBag.gv_municipalities = municipalities.Substring(0, municipalities.Length - 1);
                // Load the urls of the web api's
                loadAPIs();
                // Load the dates of the forecast
                loadMonthsCrop();
                return View();
            }
            catch(Exception ex)
            {
                return View("Error");
            }
        }
    }
}
