using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities;
using Microsoft.AspNetCore.Cors;
using System.Text;
using MongoDB.Bson;
using CIAT.DAPA.USAID.Forecast.Data.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    public class GeographicController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public GeographicController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.lc_state, LogEntity.lc_municipality, LogEntity.lc_weather_station })
        {
        }

        // GET: api/Geographic/
        [HttpGet]
        [Route("api/[controller]/Country/{format}")]
        public async Task<IActionResult> Country(string format)
        {
            try
            {
                var country = await db.country.listEnableAsync();
                List<CountryEntity> json = new List<CountryEntity>();
                CountryEntity geo_c;
                foreach (var c in country)
                {
                    geo_c = new CountryEntity() { id = c.id.ToString(), iso2 = c.iso2, name = c.name };
                    json.Add(geo_c);
                }
                // Write event log
                writeEvent("Country count [" + json.Count().ToString() + "]", LogEvent.lis);

                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "country_id", "country_iso2", "country_name", "\n" }));
                    foreach (var c in json)
                        builder.Append(string.Join<string>(delimiter, new string[] { c.id.ToString(), c.iso2, c.name, "\n" }));
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "countries.csv");
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

        // GET: api/Geographic/
        [HttpGet]
        [Route("api/[controller]/{idCountry}/{format}")]        
        public async Task<IActionResult> Get(string idCountry, string format)
        {
            try
            {
                List<StateEntity> json = new List<StateEntity>();
                var country = await db.country.byIdAsync(idCountry); ;
                var states = await db.state.listByCountryEnableAsync(country.id);
                var municipalities = await db.municipality.listEnableVisibleAsync();
                var weatherstations = await db.weatherStation.listEnableVisibleAsync();
                var crops = await db.crop.listEnableAsync();
                StateEntity geo_s;
                MunicipalityEntity geo_m;
                foreach (var s in states)
                {
                    geo_s = new StateEntity() { id = s.id.ToString(), name = s.name, country = new CountryEntity() { id = country.id.ToString(), iso2 = country.iso2, name = country.name }, municipalities = new List<MunicipalityEntity>() };
                    foreach (var m in municipalities.Where(p => p.state == s.id))
                    {
                        geo_m = new MunicipalityEntity() { id = m.id.ToString(), name = m.name, weather_stations = new List<WeatherStationEntity>() };
                        foreach (var w in weatherstations.Where(p => p.municipality == m.id))
                            geo_m.weather_stations.Add(new WeatherStationEntity()
                            {
                                id = w.id.ToString(),
                                ext_id = w.ext_id,
                                name = w.name,
                                origin = w.origin,
                                latitude = w.latitude,
                                longitude = w.longitude,
                                ranges = w.ranges.Select(p => new YieldRangeEntity()
                                {
                                    crop_id = p.crop.ToString(),
                                    label = p.label,
                                    lower = p.lower,
                                    upper = p.upper,
                                    crop_name = crops.Single(p2 => p2.id == p.crop).name
                                })
                            });
                        geo_s.municipalities.Add(geo_m);
                    }
                    json.Add(geo_s);
                }
                // Write event log
                writeEvent("Geographic count [" + json.Count().ToString() + "]", LogEvent.lis);

                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "country_name","country_iso2", "state_id", "state_name", "municipality_id", "municipality_name", "ws_id", "ws_name", "ws_origin", "ws_lat", "ws_lon", "\n" }));
                    foreach (var s in json)
                        foreach (var m in s.municipalities)
                            foreach (var w in m.weather_stations)
                                builder.Append(string.Join<string>(delimiter, new string[] { s.country.name, s.country.iso2, s.id, s.name, m.id, m.name, w.id, w.name, w.origin, w.latitude.ToString(), w.longitude.ToString(), "\n" }));
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "geographic.csv");
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

        // GET: api/Geographic/Crop/
        [HttpGet]
        [Route("api/[controller]/Crop/{idCountry}/{format}")]
        public async Task<IActionResult> GetCrop(string idCountry, string format)
        {
            try
            {
                // Get data

                List<Country> countries = await db.country.listEnableAsync();
                List<Crop> crops = await db.crop.listEnableAsync();
                // This cicle is to get all geographic information by crop
                List<CropGeographicEntity> json = new List<CropGeographicEntity>();
                foreach (Crop crop in crops)
                {
                    List<Cultivar> crops_cultivar = await db.cultivar.listByCropEnableAsync(crop.id);
                    IEnumerable<ObjectId> cultivars = crops_cultivar.Select(p => p.id);
                    var forecast = await db.forecast.getLatestAsync();

                    if (forecast != null)
                    {
                        //Get forecast yield by last forecast and cultivars
                        IEnumerable<ForecastYield> yield_forecast = await db.forecastYield.byForecastCul(forecast.id, cultivars);

                        // filter
                        List<ObjectId> ws_forecast = new List<ObjectId>();
                        // Get the weather station with data in the forecast for the crop
                        foreach (ForecastYield y in yield_forecast)
                        {
                            if (!ws_forecast.Contains(y.weather_station))
                                ws_forecast.Add(y.weather_station);

                        }
                        // Get Weather stations with municipality and state
                        List<WeatherStationAllData> ws_result = await db.weatherStation.listEnableByIDsCompleteData(ws_forecast.ToArray(), crop);
                        List<StateEntity> json_state = new List<StateEntity>();
                        StateEntity geo_s;
                        MunicipalityEntity geo_m;
                        
                        foreach (WeatherStationAllData ws_data in ws_result)
                        {

                            IEnumerable<Country> countryinstate = countries.Where(p => p.id == ws_data.std.First().depends);
                            List<Country> countryinstatelist = countryinstate.ToList();

                            // Check if state it's in json, if in get but if not in create
                            bool is_in_json = json_state.Any(p => p.id == ws_data.std.First().id.ToString());
                            if (is_in_json)
                            {
                                geo_s = json_state.Find(p => p.id == ws_data.std.First().id.ToString());
                            }
                            else
                            {
                                geo_s = new StateEntity() { id = ws_data.std.First().id.ToString(), name = ws_data.std.First().name, country = new CountryEntity() { id = countryinstatelist[0].id.ToString(), iso2 = countryinstatelist[0].iso2, name = countryinstatelist[0].name }, municipalities = new List<MunicipalityEntity>() };

                            }
                            // Check if municipality it's in state, if in get but if not in create
                            bool is_in_geo_s = geo_s.municipalities.Any(p => p.id == ws_data.munc.First().id.ToString());
                            if (is_in_geo_s)
                            {
                                geo_m = geo_s.municipalities.Find(p => p.id == ws_data.munc.First().id.ToString());
                                geo_m.weather_stations.Add(new WeatherStationEntity()
                                {
                                    id = ws_data.id.ToString(),
                                    ext_id = ws_data.ext_id,
                                    name = ws_data.name,
                                    origin = ws_data.origin,
                                    ranges = ws_data.ranges.Select(p => new YieldRangeEntity
                                    {
                                        crop_id = p.crop.ToString(),
                                        lower = p.lower,
                                        upper = p.upper,
                                        label = p.label
                                    }),
                                });
                                int index = geo_s.municipalities.FindIndex(p => p.id == ws_data.munc.First().id.ToString());
                                if (index >= 0)
                                {
                                    geo_s.municipalities[index] = geo_m;
                                }
                            }
                            else
                            {
                                geo_m = geo_m = new MunicipalityEntity() { id = ws_data.munc.First().id.ToString(), name = ws_data.munc.First().name, weather_stations = new List<WeatherStationEntity>() };
                                geo_m.weather_stations.Add(new WeatherStationEntity()
                                {
                                    id = ws_data.id.ToString(),
                                    ext_id = ws_data.ext_id,
                                    name = ws_data.name,
                                    origin = ws_data.origin,
                                    ranges = ws_data.ranges.Select(p => new YieldRangeEntity
                                    {
                                        crop_id = p.crop.ToString(),
                                        lower = p.lower,
                                        upper = p.upper,
                                        label = p.label,
                                        crop_name = crop.name
                                    }),
                                });
                                geo_s.municipalities.Add(geo_m);

                            }
                            //Add state into json if exist replace.
                            if (is_in_json)
                            {
                                int index = json_state.FindIndex(p => p.id == ws_data.std.First().id.ToString());
                                if (index >= 0)
                                {
                                    json_state[index] = geo_s;
                                }
                            }
                            else
                            {
                                json_state.Add(geo_s);
                            }

                        }
                        if (string.IsNullOrEmpty(idCountry))
                        {
                            if (json_state.Count() != 0)
                            {
                                json.Add(new CropGeographicEntity() { id = crop.id.ToString(), name = crop.name, states = json_state });
                            }
                        }
                        else
                        {
                            if (json_state.Count() != 0)
                            {
                                if (json_state[0].country.id == idCountry)
                                {
                                    json.Add(new CropGeographicEntity() { id = crop.id.ToString(), name = crop.name, states = json_state });
                                }
                            }
                        }
                    }
                }

                // Write event log
                writeEvent("Geographic forecast yield count [" + json.Count().ToString() + "]", LogEvent.lis);

                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "crop_id", "crop_name", "country_name", "country_iso2", "state_id", "state_name", "municipality_id", "municipality_name", "ws_id", "ws_name", "ws_origin", "\n" }));
                    foreach (var c in json)
                        foreach (var s in c.states)
                            foreach (var m in s.municipalities)
                                foreach (var w in m.weather_stations)
                                    builder.Append(string.Join<string>(delimiter, new string[] { c.id, c.name, s.country.name, s.country.iso2, s.id, s.name, m.id, m.name, w.id, w.name, w.origin, "\n" }));
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "geographic.csv");
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
