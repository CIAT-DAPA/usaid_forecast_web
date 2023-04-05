using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Views;
using System.IO;
using CIAT.DAPA.USAID.Forecast.Web.Models.Indicators;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class IndicadoresController : WebBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public IndicadoresController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, hostingEnvironment)
        {
        }

        // GET: /Indicadores/
        [Route("/[controller]/")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Set the parameters
                ViewBag.Section = SectionSite.Indicators;
                // Setting data
                SetWS();
                await listIndicatorsAsync();
                loadConfigurations();
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View("Error");
            }
        }

        private async Task<bool> listIndicatorsAsync()
        {
            if (IndicatorRepository.GetInstance().Indicators.Count() == 0)
                await IndicatorRepository.GetInstance().LoadAsync(hostingEnvironment.ContentRootPath + $"{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}indicators.csv");

            ViewBag.indicators_crops = IndicatorRepository.GetInstance()
                                    .Indicators.Select(p => new { CropID = p.CropID, Crop = p.Crop }).Distinct();
            ViewBag.indicators_group = IndicatorRepository.GetInstance()
                                    .Indicators.Select(p => new { GroupID = p.GroupID, Group = p.Group }).Distinct();
            ViewBag.indicators_list = IndicatorRepository.GetInstance()
                                    .Indicators.Select(p => new { CropID = p.CropID, GroupID = p.GroupID, 
                                                                IndicatorID = p.IndicatorNameID, Indicator = p.IndicatorName, 
                                                                Description= p.Description, 
                                                                Units = p.Units, Min = p.Min, Max=p.Max,
                                                                Type = p.Type, Categories= p.Categories,
                                                                Acronym = p.Acronym}).Distinct();
            
            return true;
        }

        private void loadConfigurations()
        {
            ViewBag.geoserver_url = Configurations.indicator_geoserver_url;
            ViewBag.geoserver_workspace = Configurations.indicator_geoserver_workspace;
            var period= Enumerable.Range(Configurations.indicator_geoserver_time[0], 
                                        (Configurations.indicator_geoserver_time[1] - Configurations.indicator_geoserver_time[0]) + 2)
                                    .Append(Configurations.indicator_geoserver_average)
                                    .Select(p => new { Text = (p == Configurations.indicator_geoserver_average ? "Average" : p == Configurations.indicator_geoserver_cv ? "CV" : p.ToString()), Value = p })
                                    .OrderBy(p=>p.Value);
            ViewBag.period = period;
            ViewBag.compare = (new List<string>() { "None", Configurations.indicator_NINO.ToString(), Configurations.indicator_NINA.ToString(), Configurations.indicator_geoserver_average.ToString() })
                                .Select(p=>new { Text = (Configurations.indicator_NINO.ToString() == p ? "El Niño" : (Configurations.indicator_NINA.ToString() == p ? "La Niña": (Configurations.indicator_geoserver_average.ToString() == p ? "Average" : p))), Value = p });
        }
    }
}
