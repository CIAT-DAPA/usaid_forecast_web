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
using System.Text;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities;

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

        // GET: api/Forecast/Climate/{weather_stations}/{format?}
        [HttpGet]
        [Route("api/[controller]/Climate/{weather_stations}/{probabilities}/{format}")]
        public async Task<IActionResult> GetClimate(string weather_stations, bool probabilities, string format)
        {
            try
            {
                // Transform the string id to object id
                string[] ws_parameter = weather_stations.Split(',');
                ObjectId[] ws = new ObjectId[ws_parameter.Length];
                for (int i = 0; i < ws_parameter.Length; i++)
                    ws[i] = getId(ws_parameter[i]);

                var f = await db.forecast.getLatestAsync();
                var fc = await db.forecastClimate.byForecastAndWeatherStationAsync(f.id, ws);
                var fs = await db.forecastScenario.byForecastAndWeatherStationAsync(f.id, ws);
                var json = new
                {
                    forecast = f.id.ToString(),
                    confidence = f.confidence,
                    climate = fc.Select(p => new
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
                    }),
                    scenario = fs.Select(p => new
                    {
                        weather_station = p.weather_station.ToString(),
                        name = Enum.GetName(typeof(ScenarioName), p.name),
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
                    })
                };
                // Write event log
                writeEvent("Forecast lastes climate and scenarios id " + f.id.ToString(), LogEvent.lis);
                //writeEvent(Json(json).ToJson(), LogEvent.lis);

                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // Validate the type date to export
                    if (probabilities)
                    {
                        // add header
                        builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "year", "month", "measure", "lower", "normal", "upper", "\n" }));

                        foreach (var w in json.climate)
                            foreach (var m in w.data)
                                foreach (var d in m.probabilities)
                                    builder.Append(string.Join<string>(delimiter, new string[] { w.weather_station, m.year.ToString(), m.month.ToString(), d.measure, d.lower.ToString(), d.normal.ToString(), d.upper.ToString(), "\n" }));
                    }
                    else
                    {
                        // add header
                        builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "scenario", "year", "month", "measure", "value", "\n" }));

                        foreach (var w in json.scenario)
                            foreach (var m in w.monthly_data)
                                foreach (var d in m.data)
                                    builder.Append(string.Join<string>(delimiter, new string[] { w.weather_station, w.name, w.year.ToString(), m.month.ToString(), d.measure, d.value.ToString(), "\n" }));
                    }

                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "forecast_climate_" + (probabilities ? "probabilities" : "scenarios") + ".csv");
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

        // GET: api/Forecast/Yield
        [HttpGet]
        [Route("api/[controller]/Yield/{weather_stations}/{format}")]
        public async Task<IActionResult> GetYield(string weather_stations, string format)
        {
            try
            {
                // Transform the string id to object id
                string[] ws_parameter = weather_stations.Split(',');
                ObjectId[] ws = new ObjectId[ws_parameter.Length];
                for (int i = 0; i < ws_parameter.Length; i++)
                    ws[i] = getId(ws_parameter[i]);

                var f = await db.forecast.getLatestAsync();
                
                // Write a log
                writeEvent("Forecast lastes yield id " + f.id.ToString(), LogEvent.lis);

                // Search information of forecast yield by id
                var fy_ws = await GenerateYieldForecastAsync(f, ws);
                // Build a json to return
                var json = new
                {
                    forecast = f.id.ToString(),
                    confidence = f.confidence,
                    yield = fy_ws.Select(p => new
                    {
                        weather_station = p.ws.ToString(),
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
                    })
                };
                // Validate the answer
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "cultivar_id", "soil_id", "start", "end", "measure", "median", "avg", "min", "max", "quar_1", "quar_2", "quar_3", "conf_lower", "conf_upper", "sd", "perc_5", "perc_95", "coef_var", "\n" }));

                    foreach (var w in json.yield)
                        foreach (var y in w.yield)
                            foreach (var d in y.data)
                                builder.Append(string.Join<string>(delimiter, new string[] { w.weather_station, y.cultivar, y.soil, y.start.ToString("yyyy-MM-dd"), y.end.ToString("yyyy-MM-dd"), d.measure, d.median.ToString(), d.avg.ToString(), d.min.ToString(), d.max.ToString(), d.quar_1.ToString(), d.quar_2.ToString(), d.quar_3.ToString(), d.conf_lower.ToString(), d.conf_upper.ToString(), d.sd.ToString(), d.perc_5.ToString(), d.perc_95.ToString(), d.coef_var.ToString(), "\n" }));

                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "forecast_yield.csv");
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

        /// <summary>
        /// Method which extracts information about specific forecast and list of weather stations
        /// </summary>
        /// <param name="f">Forecast</param>
        /// <param name="ws">List of weather stations</param>
        /// <returns></returns>
        private async Task<List<WeatherStationYieldEntity>> GenerateYieldForecastAsync(Data.Models.Forecast f, ObjectId[] ws)
        {
            var fy = await db.forecastYield.byForecastAndWeatherStationAsync(f.id, ws);

            // This cicle join all data by weather station
            List<WeatherStationYieldEntity> fy_ws = new List<WeatherStationYieldEntity>();
            foreach (var fy_temp in fy.Select(p => new { weather_station = p.weather_station }).Distinct())
            {
                WeatherStationYieldEntity fy_ws_temp = new WeatherStationYieldEntity() { ws = fy_temp.weather_station, yield = new List<YieldCropEntity>() };
                var yield_crop_soil_cultivar = fy.Where(p => p.weather_station == fy_temp.weather_station);
                foreach (var ycss in yield_crop_soil_cultivar)
                {
                    foreach (var yield_ycss in ycss.yield)
                        fy_ws_temp.yield.Add(new YieldCropEntity()
                        {
                            cultivar = ycss.cultivar,
                            soil = ycss.soil,
                            start = yield_ycss.start,
                            end = yield_ycss.end,
                            data = yield_ycss.data
                        });

                }
                fy_ws.Add(fy_ws_temp);
            }
            
            return fy_ws;
        }

        // GET: api/Forecast/Yield
        [HttpGet]
        [Route("api/[controller]/YieldExceedance/{weather_stations}/{format}")]
        public async Task<IActionResult> GetYieldExceedance(string weather_stations, string format)
        {
            try
            {
                // Transform the string id to object id
                string[] ws_parameter = weather_stations.Split(',');
                ObjectId[] ws = new ObjectId[ws_parameter.Length];
                for (int i = 0; i < ws_parameter.Length; i++)
                    ws[i] = getId(ws_parameter[i]);

                var f = await db.forecast.getExceedanceAsync();
                var fy = await db.forecastYield.byForecastAndWeatherStationExceedanceAsync(f.Select(p=>p.id).ToList(), ws);

                // This cicle join all data by weather station
                List<WeatherStationYieldEntity> fy_ws = new List<WeatherStationYieldEntity>();
                foreach (var fy_temp in fy.Select(p => new { weather_station = p.weather_station }).Distinct())
                {
                    WeatherStationYieldEntity fy_ws_temp = new WeatherStationYieldEntity() { ws = fy_temp.weather_station, yield = new List<YieldCropEntity>() };
                    var yield_crop_soil_cultivar = fy.Where(p => p.weather_station == fy_temp.weather_station);
                    foreach (var ycss in yield_crop_soil_cultivar)
                    {
                        foreach (var yield_ycss in ycss.yield)
                            fy_ws_temp.yield.Add(new YieldCropEntity()
                            {
                                cultivar = ycss.cultivar,
                                soil = ycss.soil,
                                start = yield_ycss.start,
                                end = yield_ycss.end,
                                data = yield_ycss.data
                            });

                    }
                    fy_ws.Add(fy_ws_temp);
                }

                // Build a json to return
                var json = new
                {
                    forecast = string.Join<string>(delimiter, f.Select(p => p.id.ToString()).ToArray()),
                    confidence = f.FirstOrDefault().confidence,
                    yield = fy_ws.Select(p => new
                    {
                        weather_station = p.ws.ToString(),
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
                    })
                };
                // Write a log
                writeEvent("Forecast lastes yield id " + string.Join<string>(delimiter,f.Select(p=>p.id.ToString()).ToArray()), LogEvent.lis);

                // Validate the answer
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "cultivar_id", "soil_id", "start", "end", "measure", "median", "avg", "min", "max", "quar_1", "quar_2", "quar_3", "conf_lower", "conf_upper", "sd", "perc_5", "perc_95", "coef_var", "\n" }));

                    foreach (var w in json.yield)
                        foreach (var y in w.yield)
                            foreach (var d in y.data)
                                builder.Append(string.Join<string>(delimiter, new string[] { w.weather_station, y.cultivar, y.soil, y.start.ToString("yyyy-MM-dd"), y.end.ToString("yyyy-MM-dd"), d.measure, d.median.ToString(), d.avg.ToString(), d.min.ToString(), d.max.ToString(), d.quar_1.ToString(), d.quar_2.ToString(), d.quar_3.ToString(), d.conf_lower.ToString(), d.conf_upper.ToString(), d.sd.ToString(), d.perc_5.ToString(), d.perc_95.ToString(), d.coef_var.ToString(), "\n" }));

                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "forecast_yield.csv");
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

        // GET: api/Forecast/SubseasonalWS/{weather_stations}/{format}
        [HttpGet]
        [Route("api/[controller]/SubseasonalWS/{weather_stations}/{format}")]
        public async Task<IActionResult> GetSubseasonalWS(string weather_stations, string format)
        {
            try
            {
                // Transform the string id to object id
                string[] ws_parameter = weather_stations.Split(',');
                ObjectId[] ws = new ObjectId[ws_parameter.Length];
                for (int i = 0; i < ws_parameter.Length; i++)
                    ws[i] = getId(ws_parameter[i]);

                var f = await db.forecast.getLatestAsync();
                var fc = await db.forecastClimate.byForecastAndWeatherStationAsync(f.id, ws);
                var json = new
                {
                    forecast = f.id.ToString(),
                    confidence = f.confidence,
                    subseasonal = fc.Select(p => new
                    {
                        weather_station = p.weather_station.ToString(),                        
                        data = p.subseasonal != null && p.subseasonal.Count() > 0 ? 
                            p.subseasonal.Select(p2 => new SubseasonalDataEntity()
                            {
                                year = p2.year,
                                month = p2.month,
                                week = p2.week,
                                probabilities = p2.probabilities.Select(p3 => new ProbabilityEntity()
                                {
                                    measure = Enum.GetName(typeof(MeasureClimatic), p3.measure),
                                    lower = p3.lower,
                                    normal = p3.normal,
                                    upper = p3.upper
                                })
                            }) : new List<SubseasonalDataEntity>()
                    }) 
                };
                // Write event log
                writeEvent("Forecast lastes subseasonal climate " + f.id.ToString(), LogEvent.lis);

                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "year", "month", "week", "measure", "lower", "normal", "upper", "\n" }));

                    foreach (var w in json.subseasonal)
                        foreach (var m in w.data)
                            foreach (var d in m.probabilities)
                                builder.Append(string.Join<string>(delimiter, new string[] { w.weather_station, m.year.ToString(), m.month.ToString(), m.week.ToString(), d.measure, d.lower.ToString(), d.normal.ToString(), d.upper.ToString(), "\n" }));

                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "forecast_subseasonal.csv");
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

        // GET: api/Forecast/Historical
        [HttpGet]
        [Route("api/[controller]/Log/{year}/{format}")]
        public async Task<IActionResult> GetLog(int year, string format)
        {
            try
            {
                var f = await db.forecast.getByYearstAsync(year);

                var json = f.Select(p => new { id = p.id.ToString(), 
                    start = p.start, 
                    end = p.end, 
                    confindece = p.confidence });

                // Write a log
                writeEvent("Forecast count  " + f.Count().ToString(), LogEvent.lis);

                // Validate the answer
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "id", "start", "end", "confidence", "\n" }));
                    // Loop for all forecast
                    foreach (var j in json)
                        builder.Append(string.Join<string>(delimiter, new string[] { j.id, j.start.ToString("yyyy-MM-dd"), j.end.ToString("yyyy-MM-dd"),  j.confindece.ToString(), "\n" }));

                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "forecast_yield.csv");
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

        // GET: api/Forecast/YieldPrevious
        [HttpGet]
        [Route("api/[controller]/YieldPrevious/{forecast}/{weather_stations}/{format}")]
        public async Task<IActionResult> GetYieldPrevious(string forecast, string weather_stations, string format)
        {
            try
            {
                // Transform the string id to object id
                string[] ws_parameter = weather_stations.Split(',');
                ObjectId[] ws = new ObjectId[ws_parameter.Length];
                for (int i = 0; i < ws_parameter.Length; i++)
                    ws[i] = getId(ws_parameter[i]);
                
                var f = await db.forecast.byIdAsync(forecast);

                // Write a log
                writeEvent("Forecast searched yield id " + f.id.ToString(), LogEvent.lis);

                // Search information of forecast yield by id
                var fy_ws = await GenerateYieldForecastAsync(f, ws);
                // Build a json to return
                var json = new
                {
                    forecast = f.id.ToString(),
                    confidence = f.confidence,
                    yield = fy_ws.Select(p => new
                    {
                        weather_station = p.ws.ToString(),
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
                    })
                };
                // Validate the answer
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "cultivar_id", "soil_id", "start", "end", "measure", "median", "avg", "min", "max", "quar_1", "quar_2", "quar_3", "conf_lower", "conf_upper", "sd", "perc_5", "perc_95", "coef_var", "\n" }));

                    foreach (var w in json.yield)
                        foreach (var y in w.yield)
                            foreach (var d in y.data)
                                builder.Append(string.Join<string>(delimiter, new string[] { w.weather_station, y.cultivar, y.soil, y.start.ToString("yyyy-MM-dd"), y.end.ToString("yyyy-MM-dd"), d.measure, d.median.ToString(), d.avg.ToString(), d.min.ToString(), d.max.ToString(), d.quar_1.ToString(), d.quar_2.ToString(), d.quar_3.ToString(), d.conf_lower.ToString(), d.conf_upper.ToString(), d.sd.ToString(), d.perc_5.ToString(), d.perc_95.ToString(), d.coef_var.ToString(), "\n" }));

                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "forecast_yield.csv");
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
