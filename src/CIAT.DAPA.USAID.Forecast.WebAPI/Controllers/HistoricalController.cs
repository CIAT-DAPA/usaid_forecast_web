using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        [Route("api/[controller]/Climatology/{weatherstations}/{format}")]
        public async Task<IActionResult> Climatology(string weatherstations, string format)
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
                // Write event log
                writeEvent("Climatology ids [" + ids + "] count: " + json.Count().ToString(), LogEvent.lis);
                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "month", "measure", "value", "\n" }));
                    foreach (var w in json)
                        foreach (var m in w.monthly_data)
                            foreach (var d in m.data)
                                builder.Append(string.Join<string>(delimiter, new string[] { w.weather_station, m.month.ToString(), d.measure, d.value.ToString(), "\n" }));
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "climatology.csv");
                }
                else
                    return Content("Format not supported");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        // GET: api/Historical/HistoricalClimatic
        [HttpGet]
        [Route("api/[controller]/HistoricalClimatic/{weatherstations}/{format}")]
        public async Task<IActionResult> HistoricalClimatic(string weatherstations, string format)
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
                // Write event log
                writeEvent("Historical climatic ids [" + ids + "] count [" + json.Count().ToString() + "]", LogEvent.lis);

                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "year", "month", "measure", "value", "\n" }));
                    foreach (var w in json)
                        foreach (var m in w.monthly_data)
                            foreach (var d in m.data)
                                builder.Append(string.Join<string>(delimiter, new string[] { w.weather_station, w.year.ToString(), m.month.ToString(), d.measure, d.value.ToString(), "\n" }));
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "historical_climatic.csv");
                }
                else
                    return Content("Format not supported");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        // GET: api/Historical/HistoricalYield
        [HttpGet]
        [Route("api/[controller]/HistoricalYieldYears/{weatherstations}/{format}")]
        public async Task<IActionResult> HistoricalYieldYears(string weatherstations, string format)
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
                // Write event log
                writeEvent("Historical yield years ids [" + ids + "] count [" + json.Count().ToString() + "]", LogEvent.lis);

                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    //builder.Append(string.Join<string>(delimiter, new string[] { "source", "ws_id", "cultivar_id", "soil_id", "start", "end", "measure", "median", "avg", "min", "max", "quar_1", "quar_2", "quar_3", "conf_lower", "conf_upper", "sd", "perc_5", "perc_95", "coef_var", "\n" }));
                    builder.Append(string.Join<string>(delimiter, new string[] { "year", "\n" }));
                    foreach (var y in json)
                        builder.Append(string.Join<string>(delimiter, new string[] { y.ToString(), "\n" }));
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "historical_yield_years.csv");
                }
                else
                    return Content("Format not supported");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }

        // GET: api/Historical/HistoricalYield
        [HttpGet]
        [Route("api/[controller]/HistoricalYield/{weatherstations}/{years}/{format}")]
        public async Task<IActionResult> HistoricalYield(string weatherstations, string years, string format)
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
                var json = (await db.historicalYield.byWeatherStationsYearsAsync(ws, y)).Select(p => new
                {
                    weather_station = p.weather_station.ToString(),
                    source = p.source.ToString(),
                    yield = p.yield.Select(p2 => new
                    {
                        soil = p.soil.ToString(),
                        cultivar = p.cultivar.ToString(),
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
                // Write event log
                writeEvent("Historical yield ids [" + weatherstations + "] count [" + json.Count().ToString() + "]", LogEvent.lis);

                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "source", "ws_id", "cultivar_id", "soil_id", "start", "end", "measure", "median", "avg", "min", "max", "quar_1", "quar_2", "quar_3", "conf_lower", "conf_upper", "sd", "perc_5", "perc_95", "coef_var", "\n" }));
                    foreach (var w in json)
                        foreach(var yi in w.yield)
                            foreach(var d in yi.data)
                                builder.Append(string.Join<string>(delimiter, new string[] { w.source, w.weather_station, yi.cultivar, yi.soil, yi.start.ToString("yyyy-MM-dd"), yi.end.ToString("yyyy-MM-dd"), d.measure, d.median.ToString(), d.avg.ToString(), d.min.ToString(), d.max.ToString(), d.quar_1.ToString(), d.quar_2.ToString(), d.quar_3.ToString(), d.conf_lower.ToString(), d.conf_upper.ToString(), d.sd.ToString(), d.perc_5.ToString(), d.perc_95.ToString(), d.coef_var.ToString(), "\n" }));
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "historical_yield.csv");
                }
                else
                    return Content("Format not supported");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }
        
    }
}
