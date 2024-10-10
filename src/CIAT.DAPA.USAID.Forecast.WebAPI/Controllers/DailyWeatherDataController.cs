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
using System.Linq;

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



        // GET: api/Historical/Climatology
        [HttpGet]
        [Route("api/[controller]/LastDailyData/{weatherStationsId}/{format}")]
        [ProducesResponseType(typeof(WeatherStationDailyDataEntity), 200)] // Suggests a successful response with HTTP status code 200
                                                                           // Suggests a response with HTTP status code 404 for not found

        public async Task<IActionResult> LastDailyData(string weatherStationsId = "651438c08a8437279ea6ca6a", string format = "json")
        {
            try
            {
                // Transform the string id to object id
                string[] parameters = weatherStationsId.Split(',');
                ObjectId[] ws = new ObjectId[parameters.Length];
                string ids = string.Empty;
                for (int i = 0; i < parameters.Length; i++)
                {
                    ws[i] = getId(parameters[i]);
                    ids += weatherStationsId[i] + ",";
                }

                IEnumerable<WeatherStationDailyData> dailyData = await db.historicalDailyData.byWeatherStationsAsync(ws);

                // Write event log
                writeEvent("Last Daily Weather Data for weather station [" + weatherStationsId + "]", LogEvent.lis);

                IEnumerable<WeatherStationDailyData> latestWeatherData = dailyData
                    .GroupBy(w => w.weather_station)  // Agrupar por estación meteorológica
                    .Select(group => group
                        .OrderByDescending(w => w.year)  // Ordenar primero por año
                        .ThenByDescending(w => w.month)  // Luego por mes
                        .ThenByDescending(w => w.daily_readings.Max(r => r.day))  // Finalmente, por día
                        .FirstOrDefault())  // Seleccionar el más reciente
                    .Where(w => w != null);

                // Evaluate the format to export
                IEnumerable<WeatherStationDailyDataJson> jsonResult = latestWeatherData.Select(data => new WeatherStationDailyDataJson()
                {
                    weather_station = data.weather_station.ToString(),
                    date = $"{data.year}/{data.month:D2}/{data.daily_readings.OrderByDescending(r => r.day).First().day:D2}",  // Formato YYYY/MM/DD
                    climaticData = data.daily_readings.OrderByDescending(r => r.day).First().data.Select(d => new ClimaticData()
                    {
                        measure = d.measure, // Mantén como enum
                        value = d.value
                    }).ToList() // Convertir a lista
                }).ToList(); // Asegúrate de convertir el resultado a lista también

                // Retornar el resultado en formato JSON
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(jsonResult);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    string delimiter = ",";  // Definir el delimitador del CSV

                    // Añadir el encabezado
                    builder.Append(string.Join(delimiter, new string[] { "ws_id", "date", "measure", "value", "\n" }));

                    // Recorrer los datos más recientes y construir las filas del CSV
                    foreach (WeatherStationDailyDataJson data in jsonResult)
                    {
                        foreach (ClimaticData climaticData in data.climaticData) // Asegúrate de usar ClimaticData aquí
                        {
                            builder.Append(string.Join(delimiter, new string[]
                            {
                                data.weather_station,                  // ws_id
                                data.date,                             // date en formato YYYY/MM/DD
                                climaticData.measure.ToString(),      // Convierte el enum a string
                                climaticData.value.ToString(),         // valor
                                "\n"                                   // Nueva línea
                            }));
                        }
                    }

                    // Convertir el StringBuilder a bytes
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());

                    // Retornar el archivo CSV
                    return File(file, "text/csv", "LastDailyData.csv");
                }
                else
                    return Content("Format not supported: " + format);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }



    }


    
}
