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
                // Set the parameters
                ViewBag.s = state ?? string.Empty;
                ViewBag.m = municipality ?? string.Empty;
                ViewBag.w = station ?? string.Empty;
                ViewBag.Section = SectionSite.Climate;
                ViewBag.climateTimeSpan = Configurations.climateTimeSpan;

                // Setting data
                SetWS();

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
                ViewBag.climate_data = forecast.Climate.FirstOrDefault()?.Data ?? new List<ForecastClimateData>();

                // Processing Scenarios data
                List <ForecastScenario> scenarios = forecast.Scenario.ToList();
                var scenario_name = scenarios.Select(p => p.Name).Distinct();
                var measures = scenarios.SelectMany(p => p.Monthly_Data).SelectMany(p => p.Data).Select(p => p.Measure).Distinct();
                List<Scenario> scenario_list = new List<Scenario>();
                foreach (var scenario in scenarios)
                {
                    foreach (var md in scenario.Monthly_Data)
                    {
                        foreach (var da in md.Data)
                        {
                            Scenario i = scenario_list.SingleOrDefault(p => p.Measure == da.Measure && p.Year == scenario.Year && p.Month == md.Month);
                            if (i == null)
                                scenario_list.Add(new Scenario()
                                {
                                    Measure = da.Measure,
                                    Year = scenario.Year,
                                    Month = md.Month,
                                    Avg = scenario.Name == "avg" ? da.Value : 0,
                                    Max = scenario.Name == "max" ? da.Value : 0,
                                    Min = scenario.Name == "min" ? da.Value : 0
                                });
                            else
                            {
                                if (scenario.Name == "avg")
                                    i.Avg = da.Value;
                                else if(scenario.Name == "max")
                                    i.Max = da.Value;
                                else if (scenario.Name == "min")
                                    i.Min = da.Value;
                            }   
                        }
                    }
                }
                ViewBag.scenarios = scenario_list;

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
                Console.WriteLine(ex);
                return View("Error");
            }
        }
    }
}
