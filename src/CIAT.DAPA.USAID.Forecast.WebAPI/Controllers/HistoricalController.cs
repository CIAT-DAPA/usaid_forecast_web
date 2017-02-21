using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    public class HistoricalController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public HistoricalController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.lc_weather_station })
        {
        }

        // GET: api/Historical/Climatology
        [HttpGet]
        [Route("api/Historical/Climatology")]
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

        // GET: api/Historical/HistoricalClimatic
        [HttpGet]
        [Route("api/Historical/HistoricalClimatic")]
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
                var json = (await db.historicalClimatic.byWeatherStationsAsync(ws)).OrderBy(p => p.year).Select(p => new
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

        // GET: api/Historical/HistoricalYield
        [HttpGet]
        [Route("api/Historical/HistoricalYieldYears")]
        public async Task<IActionResult> HistoricalYieldYears(string weatherstations)
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
                var json = (await db.historicalYield.getYearsAvailableAsync(ws));
                writeEvent("Historical yield years ids: [" + ids + "] count: " + json.Count().ToString(), LogEvent.lis);
                return Json(json);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        // GET: api/Historical/HistoricalYield
        [HttpGet]
        [Route("api/Historical/HistoricalYield")]
        public async Task<IActionResult> HistoricalYield(string weatherstations, string years)
        {
            try
            {
                // Transform the string id to object id
                string[] ws_parameter = weatherstations.Split(',');                
                ObjectId[] ws = new ObjectId[ws_parameter.Length];
                for (int i = 0; i < ws_parameter.Length; i++)
                    ws[i] = getId(ws_parameter[i]);

                // Transform the string years to int
                string[] year_parameter = years.Split(',');
                List<int> y = new List<int>();
                for (int i = 0; i < year_parameter.Length; i++)
                    y.Add(int.Parse(year_parameter[i]));

                // Search data
                var json = (await db.historicalYield.byWeatherStationsAsync(ws,y)).Select(p => new
                {
                    weather_station = p.weather_station.ToString(),
                    source = p.source,
                    yield = p.yield.Select(p2 => new
                    {
                        soil = p2.soil.ToString(),
                        cultivar = p2.cultivar.ToString(),
                        start = p2.start,
                        end = p2.end,
                        data = p2.data.Select(p3 => new
                        {
                            measure = Enum.GetName(typeof(MeasureYield), p3.measure),
                            median = p3.median,
                            avg = p3.avg,
                            min = p3.min,
                            max = p3.max,
                            quar_1 = p3.quar_1,
                            quar_2 = p3.quar_2,
                            quar_3 = p3.quar_3,
                            conf_lower = p3.conf_lower,
                            conf_upper = p3.conf_upper,
                            sd = p3.sd,
                            perc_5 = p3.perc_5,
                            perc_95 = p3.perc_95,
                            coef_var = p3.coef_var
                        })
                    })
                });
                writeEvent("Historical yield ids: [" + weatherstations + "] count: " + json.Count().ToString(), LogEvent.lis);
                return Json(json);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        // GET: api/Historical/Get
        [HttpGet]
        [Route("api/Historical/Get")]
        public async Task<IActionResult> Get(string weatherstations)
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
                var jsonClimatology = (await db.climatology.byWeatherStationsAsync(ws)).Select(p => new
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

                var jsonClimate = (await db.historicalClimatic.byWeatherStationsAsync(ws)).OrderBy(p => p.year).Select(p => new
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

                var json = new
                {
                    climatology = jsonClimatology,
                    climate = jsonClimate
                };
                writeEvent("Historical Get ids: [" + ids + "]", LogEvent.lis);
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
