using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Enums;
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
    public class RecommendationController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public RecommendationController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.cp_crop, LogEntity.cp_cultivar, LogEntity.cp_soil })
        {
        }

        // GET: api/Get
        [HttpGet]
        [Route("api/[controller]/RecommendationYield/{weather_stations}/{types}/{format}")]
        public async Task<IActionResult> RecommendationYield(string weather_stations, string types, string format)
        {
            try
            {
                // Transform the string id to object id
                string[] ws_parameter = weather_stations.Split(',');
                ObjectId[] ws = new ObjectId[ws_parameter.Length];
                for (int i = 0; i < ws_parameter.Length; i++)
                    ws[i] = getId(ws_parameter[i]);
                // Getting types of recommendations
                List<RecommendationType> types_list = new List<RecommendationType>();
                foreach (var ty in types.Split(","))
                {
                    RecommendationType tmp;
                    Enum.TryParse<RecommendationType>(ty, true, out tmp);
                    types_list.Add(tmp);
                }

                var f = await db.forecast.getLatestAsync();
                var fy = await db.forecastYield.byForecastAndWeatherStationAsync(f.id, ws);
                var weatherstations = await db.weatherStation.listEnableVisibleAsync();
                var agronomy = await db.views.listAgronomicDataAsync();


                List<RecommendationEntity> rc_ws = new List<RecommendationEntity>();
                // Loop for weather station
                foreach (var station in fy.Select(p => p.weather_station).Distinct())
                {
                    // Loop for crop
                    foreach (var crop in agronomy.Select(p => new { id = p.cp_id, name = p.cp_name, cultivars = p.cultivars.Select(p2 => new { id = p2.id, name = p2.name }), soils = p.soils.Select(p2 => new { id = p2.id, name = p2.name }) }).Distinct())
                    {
                        List<RecommendationYieldFilter> ryf_l = new List<RecommendationYieldFilter>();
                        // Filtering yield by station and soils
                        var yield_ws = fy.Where(p => p.weather_station == station && crop.soils.Select(p2 => p2.id).Contains(p.soil.ToString()));
                        foreach (var y in yield_ws)
                        {
                            foreach (var y2 in y.yield)
                                ryf_l.Add(new RecommendationYieldFilter()
                                {
                                    ws = station.ToString(),
                                    cultivar = y.cultivar.ToString(),
                                    soil = y.soil.ToString(),
                                    date = y2.start,
                                    yield = y2.data.FirstOrDefault(p => p.measure == MeasureYield.yield_0 || p.measure == MeasureYield.yield_14)
                                });
                        }

                        // Recommendation by each type of soil
                        foreach (var s in ryf_l.Select(p => p.soil).Distinct())
                        {
                            // Ordering yields for getting the best yield
                            var best = ryf_l.Where(p => p.soil.Equals(s)).OrderByDescending(p => p.yield.avg).First();
                            // Getting best cultivar and soil by them ids
                            var best_cul = crop.cultivars.Single(p => p.id == best.cultivar);
                            var best_sol = crop.soils.Single(p => p.id == s);
                            // Getting the weather station name
                            var ws_tmp = weatherstations.FirstOrDefault(p => p.id == station);
                            // Extracting keys values
                            List<RecommendationKey> keys = new List<RecommendationKey>() {
                                        new RecommendationKey(){ tag = "crop", value=crop.name, id=crop.id },
                                        new RecommendationKey(){ tag = "cultivar", value=best_cul.name, id=best_cul.id },
                                        new RecommendationKey(){ tag = "soil", value=best_sol.name, id=best_sol.id },
                                        new RecommendationKey(){ tag = "date", value=best.date.ToString("yyyy-MM-dd")},
                                        new RecommendationKey(){ tag = "yield", value=best.yield.avg.ToString()},
                                    };
                            // loop for generating recommendations
                            foreach (var t in types_list)
                            {
                                if (t == RecommendationType.best_planting_date)
                                {
                                    rc_ws.Add(new RecommendationEntity()
                                    {
                                        ws_id = station.ToString(),
                                        ws_name = ws_tmp.name,
                                        type = t,
                                        keys = keys,
                                        content = "In " + ws_tmp.name + ", " +
                                            "for the " + crop.name + " crop, " +
                                            "for plots with soils " + best_sol.name + ", " +
                                            "the best planting date is " + best.date.ToString("yyyy-MM-dd") + ", " +
                                            "with the cultivar " + best_cul.name + ", " +
                                            "likely you will get " + best.yield.avg.ToString("N0") + " kg/ha"
                                    });
                                }
                                else if (t == RecommendationType.best_cultivar)
                                {
                                    rc_ws.Add(new RecommendationEntity()
                                    {
                                        ws_id = station.ToString(),
                                        ws_name = ws_tmp.name,
                                        type = t,
                                        keys = keys,
                                        content = "In " + ws_tmp.name + ", " +
                                            "for the " + crop.name + " crop, " +
                                            "for plots with soils " + best_sol.name + ", " +
                                            "the best cultivar is " + best_cul.name + ", " +
                                            "likely you will get " + best.yield.avg.ToString("N0") + " kg/ha."
                                    });
                                }
                            }
                        }
                    }
                }


                // Write a log
                writeEvent("Recommendation lastes yield id " + f.id.ToString(), LogEvent.lis);

                // Validate the answer
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(rc_ws);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "ws_id", "ws_name", "type", "content", "keys", "\n" }));
                    foreach (var r in rc_ws)
                        builder.Append(r.ToString());
                    
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "recommendation_yield.csv");
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
