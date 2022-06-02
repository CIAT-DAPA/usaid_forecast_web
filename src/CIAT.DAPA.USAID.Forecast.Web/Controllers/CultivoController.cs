using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using Microsoft.AspNetCore.Hosting;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Views;

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

        // GET: /Cultivo/state/municipality/station/crop
        [Route("/[controller]/{state?}/{municipality?}/{station?}/{crop?}/{countryId?}")]
        public async Task<IActionResult> Index(string state, string municipality, string station, string crop, string countryId)
        {
            try
            {
                // Set the parameters
                ViewBag.s = state ?? string.Empty;
                ViewBag.m = municipality ?? string.Empty;
                ViewBag.w = station ?? string.Empty;
                ViewBag.c = crop ?? string.Empty;
                ViewBag.Section = SectionSite.Crop;

                // Setting data
                SetWS();
                ViewBag.Root = Root;

                // Searching the weather station, if the parameters don't come, it will redirect a default weather station
                if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(municipality) || string.IsNullOrEmpty(station) || string.IsNullOrEmpty(crop))
                {
                    var wsDefault = DefaultWeatherStationCrop();
                    return RedirectToAction("Index", new { wsDefault.State, wsDefault.Municipality, wsDefault.Station, wsDefault.Crop });
                }
                WeatherStationFull ws = SearchWS(state, municipality, station);
                ViewBag.ws = ws;

                // Getting the agronomic configuration
                RepositoryAgronomic rA = new RepositoryAgronomic(Root, IdCountry);
                IEnumerable<Agronomic> agronomics = await rA.ListAsync();
                Agronomic agronomic = agronomics.SingleOrDefault(p => p.Cp_Name.ToLower() == crop.ToLower());
                
                // Getting the forecast weather information
                RepositoryForecastYield rFY = new RepositoryForecastYield(Root);;
                ForecastYield forecast = await rFY.SearchAsync(ws.Id);
                ForecastYield forecast_exceedance = await rFY.SearchExceedanceAsync(ws.Id);

                IEnumerable<Yield> yield = forecast.Yield.FirstOrDefault().Yield;
                IEnumerable<Yield> yield_exceedance = forecast_exceedance.Yield.FirstOrDefault().Yield.OrderByDescending(p=>p.Data.First(p2=>p2.Measure.StartsWith("yield")).Avg);

                // Filtering cultivars
                IEnumerable<string> cultivars = yield.Select(p => p.Cultivar).Distinct().ToList();
                cultivars = cultivars.Where(p=> agronomic.Cultivars.Select(p2 => p2.Id).Distinct().Contains(p));
                ViewBag.cultivars = agronomic.Cultivars.Where(p => cultivars.Contains(p.Id));

                // Filtering soils
                IEnumerable<string> soils = yield.Select(p => p.Soil).Distinct().ToList();
                soils = soils.Where(p => agronomic.Soils.Select(p2 => p2.Id).Distinct().Contains(p));
                ViewBag.soils = agronomic.Soils.Where(p => soils.Contains(p.Id)).AsEnumerable();
                
                //
                ViewBag.agronomic = agronomic;
                ViewBag.yield = yield;
                ViewBag.yield_exceedance = yield_exceedance;
                ViewBag.ranges = ws.Ranges.Where(p => p.Crop_Name.ToLower() == crop.ToLower());
                

                return View();
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
    }
}
