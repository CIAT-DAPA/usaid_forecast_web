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

        // GET: api/Get
        [HttpGet]
        [Route("api/[controller]/{format?}")]
        public async Task<IActionResult> Get(string format)
        {
            try
            {
                var states = await db.state.listEnableAsync();
                var municipalities = await db.municipality.listEnableVisibleAsync();
                var weatherstations = await db.weatherStation.listEnableVisibleAsync();
                var crops = await db.crop.listEnableAsync();
                List<StateEntity> json = new List<StateEntity>();
                StateEntity geo_s;
                MunicipalityEntity geo_m;
                foreach (var s in states)
                {
                    geo_s = new StateEntity() { id = s.id.ToString(), name = s.name, country = s.country.name, municipalities = new List<MunicipalityEntity>() };
                    foreach (var m in municipalities.Where(p => p.state == s.id))
                    {
                        geo_m = new MunicipalityEntity() { id = m.id.ToString(), name = m.name, weather_stations = new List<WeatherStationEntity>() };
                        foreach (var w in weatherstations.Where(p => p.municipality == m.id))
                            geo_m.weather_stations.Add(new WeatherStationEntity()
                            {
                                id = w.id.ToString(),
                                name = w.name,
                                origin = w.origin,
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
                writeEvent(json.Count().ToString(), LogEvent.lis);
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header
                    builder.Append(string.Join<string>(delimiter, new string[] { "country_name", "state_id", "state_name", "municipality_id", "municipality_name", "ws_id", "ws_name", "ws_origin", "\n" }));
                    foreach (var s in json)
                        foreach (var m in s.municipalities)
                            foreach (var w in m.weather_stations)
                                builder.Append(string.Join<string>(delimiter, new string[]{ s.country, s.id, s.name, m.id, m.name, w.id, w.name, w.origin, "\n" }));
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
