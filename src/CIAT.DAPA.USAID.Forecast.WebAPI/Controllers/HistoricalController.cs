using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    
    public class HistoricalController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public HistoricalController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.cp_crop })
        {
        }
        /*
        // GET: api/Get
        [HttpGet]
        [Route("api/[controller]/climatology")]
        public async Task<IActionResult> Climatology(string[] weatherstations)
        {
            try
            {
                // Transform the string id to object id
                ObjectId[] ws = new ObjectId[weatherstations.Length];
                for (int i = 0; i < weatherstations.Length; i++)
                    ws[i] = getId(weatherstations[i]);
                var json = await db.views.climatologyByStations(ws);
                writeEvent(json.Count().ToString(), LogEvent.lis);
                return Json(json);
            }
            catch(Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }*/
    }
}
