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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Get()
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
