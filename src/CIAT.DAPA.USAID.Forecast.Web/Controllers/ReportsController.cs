using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Web.Models.Tools;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Views;

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class ReportsController : WebBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public ReportsController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, hostingEnvironment)
        {
        }


        // GET: /Reports/state/municipality/station
        [Route("/[controller]/{state?}/{municipality?}/{station?}")]
        public async Task<IActionResult> Index(string state, string municipality, string station)
        {
            try
            {


 

                // Setting data
                SetWS();



                // Searching the weather station, if the parameters don't come, it will redirect a default weather station                
                if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(municipality) || string.IsNullOrEmpty(station))
                {
                    WeatherStationFull wsDefault = DefaultWeatherStation();
                    return RedirectToAction("Index", new { wsDefault.State, wsDefault.Municipality, wsDefault.Station });
                }
                WeatherStationFull weatherStation = SearchWS(state, municipality, station);
                Console.WriteLine("weather station:");
                Console.WriteLine(weatherStation.Latitude);
                ViewBag.weatherStation = weatherStation;


                // Getting the forecast weather information
                RepositoryForecastWeather rFW = new RepositoryForecastWeather(Root);
                ForecastWeather weatherForecast = await rFW.SearchAsync(weatherStation.Id);
                ViewBag.weatherForecast = weatherForecast;

     
                // Getting historical climate
                RepositoryHistoricalClimatology rHCy = new RepositoryHistoricalClimatology(Root);
                ViewBag.historicalWeatherData = (await rHCy.SearchAsync(weatherStation.Id)).FirstOrDefault();


                // Getting the agronomic configuration
                RepositoryAgronomic rA = new RepositoryAgronomic(Root, IdCountry);
                IEnumerable<Agronomic> agronomics = await rA.ListAsync();
                ViewBag.agronomics = agronomics;

                // Getting the forecast weather information
                RepositoryForecastYield rFY = new RepositoryForecastYield(Root); ;
                ForecastYield yieldForecast = await rFY.SearchAsync(weatherStation.Id);
                ViewBag.yieldForecast = yieldForecast;
              


                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View("Error");
            }
        }


/*
        public IActionResult DownloadPdf()
        {
            var pdfBytes = GeneratePdf(); // Implement GeneratePdf method to create the PDF
            return File(pdfBytes, "application/pdf", "report.pdf");
        }

        private byte[] GeneratePdf()
        {
            // Implement PDF generation logic here using DinkToPdf or any other library
        }
        */

    }
}
