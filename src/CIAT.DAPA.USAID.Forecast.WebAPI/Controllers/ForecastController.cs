using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using Microsoft.AspNetCore.Cors;
using MongoDB.Bson;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    public class ForecastController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public ForecastController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.fs_forecast })
        {

        }

        // GET: api/[controller]
        [HttpGet]
        [Route("api/Forecast/Full")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var f = await db.forecast.getLatestAsync();
                var json = new
                {
                    forecast = f.id.ToString(),
                    confidence = f.confidence,
                    climate = await getForecastClimate(f.id),
                    yield = await getForecastYield(f.id)
                };
                writeEvent("Forecast lastes", LogEvent.lis);
                return Json(json);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        // GET: api/[controller]
        [HttpGet]
        [Route("api/Forecast/Climate")]
        public async Task<IActionResult> GetClimate()
        {
            try
            {
                var f = await db.forecast.getLatestAsync();
                var fc = await db.forecastClimate.byForecastAsync(f.id);
                var json = new
                {
                    forecast = f.id.ToString(),
                    confidence = f.confidence,
                    climate = await getForecastClimate(f.id)
                };
                writeEvent("Forecast lastes climate", LogEvent.lis);
                return Json(json);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        // GET: api/[controller]
        [HttpGet]
        [Route("api/Forecast/Yield")]
        public async Task<IActionResult> GetYield()
        {
            try
            {
                var f = await db.forecast.getLatestAsync();

                var json = new
                {
                    forecast = f.id.ToString(),
                    confidence = f.confidence,
                    yield = await getForecastYield(f.id)
                };
                writeEvent("Forecast lastes", LogEvent.lis);
                return Json(json);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        private async Task<object> getForecastClimate(ObjectId forecast)
        {
            var fc = await db.forecastClimate.byForecastAsync(forecast);
            return fc.Select(p => new
            {
                weather_station = p.weather_station.ToString(),
                performance = p.performance.Select(p2 => new
                {
                    measure = Enum.GetName(typeof(MeasurePerformance), p2.name),
                    value = p2.value,
                    year = p2.year,
                    month = p2.month
                }),
                data = p.data.Select(p2 => new
                {
                    year = p2.year,
                    month = p2.month,
                    probabilities = p2.probabilities.Select(p3 => new
                    {
                        measure = Enum.GetName(typeof(MeasureClimatic), p3.measure),
                        lower = p3.lower,
                        normal = p3.normal,
                        upper = p3.upper
                    })
                })
            });
        }

        private async Task<object> getForecastYield(ObjectId forecast)
        {
            var fy = await db.forecastYield.byForecastAsync(forecast);
            return fy.Select(p => new
            {
                weather_station = p.weather_station.ToString(),
                yield = p.yield.Select(p2 => new
                {
                    cultivar = p2.cultivar.ToString(),
                    soil = p2.soil.ToString(),
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
        }

    }
}
