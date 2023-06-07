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
        [Route("api/[controller]/RecommendationYield/{weather_stations}/{language}/{format}")]
        public async Task<IActionResult> RecommendationYield(string weather_stations, string language, string format)
        {
            try
            {
                // Transform the string id to object id
                string[] ws_parameter = weather_stations.Split(',');
                ObjectId[] ws = new ObjectId[ws_parameter.Length];
                for (int i = 0; i < ws_parameter.Length; i++)
                    ws[i] = getId(ws_parameter[i]);
                // Getting types of recommendations


                var f = await db.forecast.getLatestAsync();
                var fy = await db.forecastYield.byForecastAndWeatherStationAsync(f.id, ws);
                var weatherstations = await db.weatherStation.listEnableVisibleAsync();
                var agronomy = await db.views.listAgronomicDataAsync();
                List<Country> countries = await db.country.listEnableAsync();
                List<State> states = await db.state.listEnableAsync();
                List<Municipality> muni = await db.municipality.listEnableAsync();


                List<RecommendationEntity> rc_ws = new List<RecommendationEntity>();
                DateTime today = DateTime.Today;

                RecommendationLang recommendation_lang;

                if (Enum.IsDefined(typeof(RecommendationLang), language))
                {
                    Enum.TryParse(language, out recommendation_lang);
                }
                else
                {
                    recommendation_lang = RecommendationLang.eng;
                }



                // Loop for weather station
                foreach (var station in fy.Select(p => p.weather_station).Distinct())
                {
                    WeatherStation ws_tmp = weatherstations.FirstOrDefault(p => p.id == station);
                    ObjectId country_id = countries.FirstOrDefault(c => c.id == states.FirstOrDefault(s => s.id == muni.FirstOrDefault(m => m.id == ws_tmp.municipality).state ).country).id;
                    bool is_inseason = false;
                    // Loop for crop
                    foreach (var crop in agronomy.Select(p => new { id = p.cp_id, name = p.cp_name, cultivars = p.cultivars.Select(p2 => new { id = p2.id, name = p2.name }), soils = p.soils.Select(p2 => new { id = p2.id, name = p2.name }) }).Distinct())
                    {
                        List<RecommendationYieldFilter> ryf_l = new List<RecommendationYieldFilter>();
                        List<RecommendationYieldFilter> ryf_land = new List<RecommendationYieldFilter>();
                        Crop crop_data = await db.crop.byIdAsync(crop.id);
                        // Filtering yield by station and soils
                        IEnumerable<ForecastYield> yield_ws = fy.Where(p => p.weather_station == station && crop.soils.Select(p2 => p2.id).Contains(p.soil.ToString()));
                        
                        foreach (var yield in yield_ws)
                        {
                            foreach(YieldCrop yield_crop in yield.yield)
                            {
                                // Pre Season
                                if ((today - yield_crop.start).TotalDays < 0)
                                {
                                    ryf_l.Add(new RecommendationYieldFilter()
                                    {
                                        ws = station.ToString(),
                                        cultivar = yield.cultivar.ToString(),
                                        soil = yield.soil.ToString(),
                                        date = yield_crop.start,
                                        yield = yield_crop.data.FirstOrDefault(p => p.measure == MeasureYield.yield_0 || p.measure == MeasureYield.yield_14 || p.measure == MeasureYield.land_pre_day)
                                    });

                                     IEnumerable<YieldData> yield_data_list = yield_crop.data.Where(p => p.measure == MeasureYield.land_pre_day);
                                    
                                    if (yield_data_list.Count() > 0)
                                    {
                                        YieldData yield_data = yield_data_list.OrderByDescending(p => p.avg).First();
                                        ryf_land.Add(new RecommendationYieldFilter()
                                        {
                                            ws = station.ToString(),
                                            cultivar = yield.cultivar.ToString(),
                                            soil = yield.soil.ToString(),
                                            date = yield_crop.start,
                                            end_date = yield_crop.end,
                                            yield = yield_data
                                        });
                                    }   
                                }
                                else if ((today - yield_crop.start).TotalDays >= 0 && (today - yield_crop.end).TotalDays < 0)
                                {
                                    IEnumerable<ForecastPhenPhase> phen_phase_list = await db.forecastPhenPhase.byIndexAsync(f.id, yield.cultivar, yield.weather_station, yield.soil);
                                    List<string> phases = new List<string>();
                                    foreach (ForecastPhenPhase phen_phase in phen_phase_list)
                                    {
                                        IEnumerable<PhaseCrop> ph_crop_list = phen_phase.phases_crop.Where(p => (today >= p.start) && (today <= p.end));
                                        foreach (PhaseCrop ph_crop in ph_crop_list)
                                        {
                                            IEnumerable<PhaseData> ph_data_list = ph_crop.data.Where(p => (today >= p.start) && (today <= p.end));
                                            foreach (PhaseData ph_data in ph_data_list)
                                            {
                                                phases.Add(ph_data.name);
                                            }
                                        }
                                    }


                                    IEnumerable<YieldData> yield_data_list = yield_crop.data.Where(p => phases.Contains(p.measure.ToString().Replace("hs_", "").Replace("st_", "").Replace("_w", "").Replace("_n", "")));
                                    foreach (YieldData yield_data in yield_data_list)
                                    {
                                        string type_of_recommendation = crop_data.crop_config.Where(cr => cr.label == yield_data.measure.ToString().Replace("hs_", "").Replace("st_", "").Replace("_w", "").Replace("_n", "")).First().type;
                                        if (type_of_recommendation == "st")
                                        {
                                            string type = yield_data.measure.ToString().Split("_")[yield_data.measure.ToString().Split("_").Length - 1];

                                            // Getting best cultivar and soil by them ids
                                            var best_cul = crop.cultivars.Single(p => p.id == yield.cultivar.ToString());
                                            var best_sol = crop.soils.Single(p => p.id == yield.soil.ToString());
                                            List<RecommendationKey> keys = new List<RecommendationKey>() {
                                                        new RecommendationKey(){ tag = "crop", value=crop.name, id=crop.id },
                                                        new RecommendationKey(){ tag = "cultivar", value=best_cul.name, id=best_cul.id },
                                                        new RecommendationKey(){ tag = "soil", value=best_sol.name, id=best_sol.id },
                                                        new RecommendationKey(){ tag = "date", value=yield_crop.start.ToString("yyyy-MM-dd")},
                                                        new RecommendationKey(){ tag = "yield", value=yield_data.avg.ToString()},
                                                        new RecommendationKey(){ tag = "advisory", value=AdvisoryType.in_season.ToString()},
                                                    };
                                            RecommendationEntity recommentdation = new RecommendationEntity()
                                            {
                                                ws_id = station.ToString(),
                                                ws_name = ws_tmp.name,
                                                type = yield_data.measure.ToString(),
                                                keys = keys,

                                            };



                                            List<VarReplace> replaces = new List<VarReplace>();

                                            replaces.Add(new VarReplace("ws_name", ws_tmp.name));
                                            replaces.Add(new VarReplace("crop_name", crop.name));
                                            replaces.Add(new VarReplace("sol_name", best_sol.name));


                                            List<Recommendation> phase_pheno_msgs = new List<Recommendation>();

                                            if (yield_data.avg > 0.66)
                                            {
                                                phase_pheno_msgs = (List<Recommendation>)await db.recommendation.byIndexAsync(country_id, yield_data.measure.ToString(), Data.Enums.RecommendationType.above_normal.ToString(), recommendation_lang);


                                            }
                                            else if (yield_data.avg > 0.33)
                                            {
                                                phase_pheno_msgs = (List<Recommendation>)await db.recommendation.byIndexAsync(country_id, yield_data.measure.ToString(), Data.Enums.RecommendationType.normal.ToString(), recommendation_lang);  
                                            }
                                            else
                                            {
                                                phase_pheno_msgs = (List<Recommendation>)await db.recommendation.byIndexAsync(country_id, yield_data.measure.ToString(), Data.Enums.RecommendationType.below_normal.ToString(), recommendation_lang);
                                                
                                            }

                                            if (phase_pheno_msgs.Count() > 0)
                                            {
                                                string phase_pheno_msg = phase_pheno_msgs.First().resp;


                                                string new_phase_pheno_msg = VarReplace.createNewMsg(replaces, phase_pheno_msg);
                                                recommentdation.content = new_phase_pheno_msg;
                                            }
                                            else
                                            {
                                                recommentdation.content = "It is necessary to add a recommendation for this phenological phase: " + yield_data.measure.ToString() + " and this langauges: " + recommendation_lang;
                                            }

                                            is_inseason = true;
                                            rc_ws.Add(recommentdation);
                                        }
                                        else if(type_of_recommendation == "hs")
                                        {
                                            var best_cul = crop.cultivars.Single(p => p.id == yield.cultivar.ToString());
                                            var best_sol = crop.soils.Single(p => p.id == yield.soil.ToString());
                                            List<RecommendationKey> keys = new List<RecommendationKey>() {
                                                        new RecommendationKey(){ tag = "crop", value=crop.name, id=crop.id },
                                                        new RecommendationKey(){ tag = "cultivar", value=best_cul.name, id=best_cul.id },
                                                        new RecommendationKey(){ tag = "soil", value=best_sol.name, id=best_sol.id },
                                                        new RecommendationKey(){ tag = "date", value=yield_crop.start.ToString("yyyy-MM-dd")},
                                                        new RecommendationKey(){ tag = "yield", value=yield_data.avg.ToString()},
                                                        new RecommendationKey(){ tag = "advisory", value=AdvisoryType.in_season.ToString()},
                                                    };
                                            RecommendationEntity recommentdation = new RecommendationEntity()
                                            {
                                                ws_id = station.ToString(),
                                                ws_name = ws_tmp.name,
                                                type = yield_data.measure.ToString(),
                                                keys = keys,

                                            };



                                            List<VarReplace> replaces = new List<VarReplace>();

                                            replaces.Add(new VarReplace("ws_name", ws_tmp.name));
                                            replaces.Add(new VarReplace("crop_name", crop.name));
                                            replaces.Add(new VarReplace("sol_name", best_sol.name));


                                            IEnumerable<Recommendation> climate_msgs = await db.recommendation.byIndexAsync(country_id, yield_data.measure.ToString(), Data.Enums.RecommendationType.climate.ToString(), recommendation_lang);


                                            if (climate_msgs.Count() > 0)
                                            {
                                                string climate_msg = climate_msgs.First().resp;


                                                string new_climate_msg = VarReplace.createNewMsg(replaces, climate_msg);
                                                recommentdation.content = climate_msg;
                                            }
                                            else
                                            {
                                                recommentdation.content = "It is necessary to add a recommendation for this phenological phase: " + yield_data.measure.ToString() + " and this langauges: " + recommendation_lang;
                                            }


                                            is_inseason = true;
                                            rc_ws.Add(recommentdation);
                                        }
                                        
                                    }
                                }
                            }
                                 
                        }


                        if (ryf_l.Count() > 0 && !is_inseason)
                        {
                            // Recommendation by each type of soil
                            foreach (var s in ryf_l.Select(p => p.soil).Distinct())
                            {
                                // Ordering yields for getting the best yield
                                var best = ryf_l.Where(p => p.soil.Equals(s)).OrderByDescending(p => p.yield.avg).First();
                                // Getting best cultivar and soil by them ids
                                var best_cul = crop.cultivars.Single(p => p.id == best.cultivar);
                                var best_sol = crop.soils.Single(p => p.id == s);
                                // Getting the weather station name
                                // Extracting keys values
                                List<RecommendationKey> keys = new List<RecommendationKey>() {
                                    new RecommendationKey(){ tag = "crop", value=crop.name, id=crop.id },
                                    new RecommendationKey(){ tag = "cultivar", value=best_cul.name, id=best_cul.id },
                                    new RecommendationKey(){ tag = "soil", value=best_sol.name, id=best_sol.id },
                                    new RecommendationKey(){ tag = "date", value=best.date.ToString("yyyy-MM-dd")},
                                    new RecommendationKey(){ tag = "yield", value=best.yield.avg.ToString()},
                                    new RecommendationKey(){ tag = "advisory", value=AdvisoryType.pre_season.ToString()},
                                };

                                IEnumerable<Recommendation> planting_day_msgs = await db.recommendation.byIndexAsync(country_id, "best_planting_date", Data.Enums.RecommendationType.pre_season.ToString(), recommendation_lang);

                                string planting_day_msg = "";

                                if (planting_day_msgs.Count() > 0)
                                {
                                    planting_day_msg = planting_day_msgs.First().resp;

                                }
                                else
                                {
                                    planting_day_msg = "It is necessary to add a recommendation for best planting date" + " and this langauges: " + recommendation_lang;
                                }

                                

                                List<VarReplace> replaces = new List<VarReplace>();

                                replaces.Add(new VarReplace("ws_name", ws_tmp.name));
                                replaces.Add(new VarReplace("crop_name", crop.name));
                                replaces.Add(new VarReplace("sol_name", best_sol.name));
                                replaces.Add(new VarReplace("best_date", best.date.ToString("yyyy-MM-dd")));
                                replaces.Add(new VarReplace("cul_name", best_cul.name));
                                replaces.Add(new VarReplace("best_yield", best.yield.avg.ToString("N0")));


                                string new_planting_day_msg = VarReplace.createNewMsg(replaces, planting_day_msg);


                                rc_ws.Add(new RecommendationEntity()
                                {
                                    ws_id = station.ToString(),
                                    ws_name = ws_tmp.name,
                                    type = Models.Enums.RecommendationType.best_planting_date.ToString(),
                                    keys = keys,
                                    content = new_planting_day_msg
                                });

                                IEnumerable<Recommendation> best_cultivar_msgs = await db.recommendation.byIndexAsync(country_id, "best_cultivar", Data.Enums.RecommendationType.pre_season.ToString(), recommendation_lang);


                                string best_cultivar_msg = "";

                                if (best_cultivar_msgs.Count() > 0)
                                {
                                    best_cultivar_msg = best_cultivar_msgs.First().resp;

                                }
                                else
                                {
                                    best_cultivar_msg = "It is necessary to add a recommendation for best cultivar" + " and this langauges: " + recommendation_lang;
                                }



                                string new_best_cultivar_msg = VarReplace.createNewMsg(replaces, best_cultivar_msg);


                                rc_ws.Add(new RecommendationEntity()
                                {
                                    ws_id = station.ToString(),
                                    ws_name = ws_tmp.name,
                                    type = Models.Enums.RecommendationType.best_cultivar.ToString(),
                                    keys = keys,
                                    content = new_best_cultivar_msg
                                });


                            }
                        }
                        

                        if(ryf_land.Count() > 0 && !is_inseason)
                        {
                            foreach (var s in ryf_land.Select(p => p.soil).Distinct())
                            {
                                RecommendationYieldFilter best_land = ryf_land.Where(p => p.soil.Equals(s)).OrderByDescending(p => p.yield.avg).First();
                                var best_cul_land = crop.cultivars.Single(p => p.id == best_land.cultivar);
                                var best_sol_land = crop.soils.Single(p => p.id == s);
                                // Getting the weather station name
                                var ws_tmp_land = weatherstations.FirstOrDefault(p => p.id == station);
                                // Extracting keys values
                                List<RecommendationKey> keys_land = new List<RecommendationKey>() {
                                        new RecommendationKey(){ tag = "crop", value=crop.name, id=crop.id },
                                        new RecommendationKey(){ tag = "cultivar", value=best_cul_land.name, id=best_cul_land.id },
                                        new RecommendationKey(){ tag = "soil", value=best_sol_land.name, id=best_sol_land.id },
                                        new RecommendationKey(){ tag = "date", value=best_land.date.ToString("yyyy-MM-dd")},
                                        new RecommendationKey(){ tag = "yield", value=best_land.yield.avg.ToString()},
                                        new RecommendationKey(){ tag = "advisory", value=AdvisoryType.pre_season.ToString()},
                                    };


                                IEnumerable<Recommendation> land_pre_msgs = await db.recommendation.byIndexAsync(country_id, "land_pre_day", Data.Enums.RecommendationType.pre_season.ToString(), recommendation_lang);


                                string land_pre_msg = "";

                                if (land_pre_msgs.Count() > 0)
                                {
                                    land_pre_msg = land_pre_msgs.First().resp;

                                }
                                else
                                {
                                    land_pre_msg = "It is necessary to add a recommendation for best cultivar" + " and this langauges: " + recommendation_lang;
                                }


                                List<VarReplace> replaces = new List<VarReplace>();

                                replaces.Add(new VarReplace("ws_name", ws_tmp.name));
                                replaces.Add(new VarReplace("crop_name", crop.name));
                                replaces.Add(new VarReplace("sol_name", best_sol_land.name));
                                replaces.Add(new VarReplace("best_start_date", best_land.date.ToString("yyyy-MM-dd")));
                                replaces.Add(new VarReplace("cul_name", best_cul_land.name));
                                replaces.Add(new VarReplace("best_end_date", best_land.end_date.ToString("yyyy-MM-dd")));


                                string new_planting_day_msg = VarReplace.createNewMsg(replaces, land_pre_msg);

                                rc_ws.Add(new RecommendationEntity()
                                {
                                    ws_id = station.ToString(),
                                    ws_name = ws_tmp_land.name,
                                    type = Models.Enums.RecommendationType.land_pre_day.ToString(),
                                    keys = keys_land,
                                    content = new_planting_day_msg
                                });
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
