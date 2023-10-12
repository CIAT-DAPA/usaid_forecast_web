using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    public class DailyWeatherDataController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public DailyWeatherDataController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.lc_weather_station })
        {
   
        }

        // GET: api/Historical/Climatology
        [HttpGet]
        [Route("api/[controller]/Climatology/{weatherStationId}/{format}")]
        [ProducesResponseType(typeof(WeatherStationDailyDataEntity), 200)] // Suggests a successful response with HTTP status code 200
         // Suggests a response with HTTP status code 404 for not found
     
        public async Task<IActionResult> Climatology(string weatherStationId = "651438a68a8437279ea6ca46", [FromQuery] int year = 2023,
        [FromQuery] int month = 10, string format = "json")
        {
            try
            {
        
                WeatherStationDailyData dailyData = await db.historicalDailyData.GetDailyDataForWeatherStationAsync(weatherStationId,year,month);
                
                // Write event log
                writeEvent("Daily Weather Data Climatology for weather station [" + weatherStationId + "] retrieved: " + dailyData.id.ToString(), LogEvent.lis);
                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(dailyData);
                else if (format.ToLower().Trim().Equals("csv"))
                {


                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "ws_id","year", "month", "day", "measure", "value", "\n" }));
                    foreach (var dailyReading in dailyData.daily_readings)
                        foreach (var data in dailyReading.data)
                                builder.Append(string.Join<string>(delimiter, new string[] { 
                                    dailyData.weather_station.ToString(),
                                      dailyData.year.ToString(),
                                    dailyData.month.ToString(),
                                       dailyReading.day.ToString(),
                                    data.measure.ToString(), 
                                    data.value.ToString(), 
                                    "\n" 
                                }));
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    
                    return File(file, "text/csv", "climatology.csv");
                    
              
                }
                else
                    return Content("Format not supported: "+ format);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

       
        
    }


    
}
