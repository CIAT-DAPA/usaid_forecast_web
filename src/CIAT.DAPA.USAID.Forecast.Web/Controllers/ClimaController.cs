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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class ClimaController : WebBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public ClimaController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, hostingEnvironment)
        {
        }

        // GET: /Clima/state/municipality/station
        [Route("/[controller]/{state?}/{municipality?}/{station?}")]
        public async Task<IActionResult> Index(string state, string municipality, string station)
        {
            try
            {
                // Load the urls of the web api's
                loadAPIs();                
                // Set the parameters
                ViewBag.s = state ?? string.Empty;
                ViewBag.m = municipality ?? string.Empty;
                ViewBag.w = station ?? string.Empty;
                ViewBag.Section = SectionSite.Climate;

                // Searching the weather station, if the parameters don't come, it will redirect a default weather station                
                if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(municipality) || string.IsNullOrEmpty(station))
                {
                    WeatherStationFull wsDefault = DefaultWeatherStation();
                    return RedirectToAction("Index", new { wsDefault.State, wsDefault.Municipality, wsDefault.Station });
                }
                WeatherStationFull ws = SearchWS(state, municipality, station);
                ViewBag.ws = ws;

                // Getting the forecast weather information
                RepositoryForecastWeather rFW = new RepositoryForecastWeather(Root);
                ForecastWeather forecast = await rFW.SearchAsync(ws.Id);
                // Sending the climate data to the view
                ViewBag.climate_data = forecast.Climate.First().Data;
                // Processing Scenarios data
                List<ForecastScenario> scenario = forecast.Scenario.ToList();
                var scenario_name = scenario.Select(p => p.Name).Distinct();
                var measures = scenario.SelectMany(p => p.Monthly_Data).SelectMany(p => p.Data).Select(p => p.Measure).Distinct();
                List<Scenario> scenario_list = new List<Scenario>();
                foreach (var s in scenario)
                {
                    foreach (var md in s.Monthly_Data)
                    {
                        foreach (var da in md.Data)
                        {
                            Scenario i = scenario_list.SingleOrDefault(p => p.Measure == da.Measure && p.Year == s.Year && p.Month == md.Month);
                            if (i == null)
                                scenario_list.Add(new Scenario()
                                {
                                    Measure = da.Measure,
                                    Year = s.Year,
                                    Month = md.Month,
                                    Avg = s.Name == "avg" ? da.Value : 0,
                                    Max = s.Name == "max" ? da.Value : 0,
                                    Min = s.Name == "min" ? da.Value : 0
                                });
                            else
                            {
                                if (s.Name == "avg")
                                    i.Avg = da.Value;
                                else if(s.Name == "max")
                                    i.Max = da.Value;
                                else if (s.Name == "min")
                                    i.Min = da.Value;
                            }   
                        }
                    }
                }
                ViewBag.scenario = scenario_list;

                // Getting de historical climate
                RepositoryHistoricalClimate rHC = new RepositoryHistoricalClimate(Root);
                ViewBag.historical = await rHC.SearchAsync(ws.Id);

                // Getting climatology
                RepositoryHistoricalClimatology rHCy = new RepositoryHistoricalClimatology(Root);
                ViewBag.climatology = (await rHCy.SearchAsync(ws.Id)).FirstOrDefault();

                // Settings variables
                ViewBag.weather_vars = new string[] { "prec", "sol_rad", "t_max", "t_min" };

                return View();
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
    }
}
