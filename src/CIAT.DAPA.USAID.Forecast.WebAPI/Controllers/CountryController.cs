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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    public class CountryController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public CountryController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.lc_state, LogEntity.lc_municipality, LogEntity.lc_weather_station })
        {
        }

        // GET: api/Geographic/
        [HttpGet]
        [Route("api/[controller]")]
        public async Task<IActionResult> Get()
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
