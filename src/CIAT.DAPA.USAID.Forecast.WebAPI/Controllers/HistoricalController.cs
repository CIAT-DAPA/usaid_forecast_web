using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{

    public class HistoricalController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public HistoricalController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.cp_crop })
        {
        }

        // GET: api/Get
        [HttpGet]
        [Route("api/[controller]/Climatology")]
        public async Task<IActionResult> Climatology(string weatherstations)
        {
            try
            {
                // Transform the string id to object id
                string[] parameters = weatherstations.Split(',');
                ObjectId[] ws = new ObjectId[parameters.Length];
                string ids = string.Empty;
                for (int i = 0; i < parameters.Length; i++)
                {
                    ws[i] = getId(parameters[i]);
                    ids += weatherstations[i] + ",";
                }
                var json = (await db.climatology.byWeatherStationsAsync(ws)).Select(p => new
                {
                    weather_station = p.weather_station.ToString(),
                    monthly_data = p.monthly_data.Select(p2 => new
                    {
                        month = p2.month,
                        data = p2.data.Select(p3 => new
                        {
                            measure = Enum.GetName(typeof(MeasureClimatic), p3.measure),
                            value = p3.value
                        })
                    })
                });
                writeEvent("Climatology ids: [" + ids + "] count: " + json.Count().ToString(), LogEvent.lis);
                return Json(json);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        // GET: api/Get
        [HttpGet]
        [Route("api/[controller]/HistoricalClimatic")]
        public async Task<IActionResult> HistoricalClimatic(string weatherstations)
        {
            try
            {
                // Transform the string id to object id
                string[] parameters = weatherstations.Split(',');
                ObjectId[] ws = new ObjectId[parameters.Length];
                string ids = string.Empty;
                for (int i = 0; i < parameters.Length; i++)
                {
                    ws[i] = getId(parameters[i]);
                    ids += weatherstations[i] + ",";
                }                
                var json = (await db.historicalClimatic.byWeatherStationsAsync(ws)).Select(p => new
                {
                    weather_station = p.weather_station.ToString(),
                    year = p.year,
                    monthly_data = p.monthly_data.Select(p2 => new
                    {
                        month = p2.month,
                        data = p2.data.Select(p3 => new
                        {
                            measure = Enum.GetName(typeof(MeasureClimatic), p3.measure),
                            value = p3.value
                        })
                    })
                });
                writeEvent("Historical climatic ids: [" + ids + "] count: " + json.Count().ToString(), LogEvent.lis);
                return Json(json);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }
    }
}
