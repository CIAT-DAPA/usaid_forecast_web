using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    public class AgronomicController : WebAPIBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Configuration options</param>
        public AgronomicController(IOptions<Settings> settings) : base(settings, new List<LogEntity>() { LogEntity.cp_crop, LogEntity.cp_cultivar, LogEntity.cp_soil })
        {
        }

        // GET: api/Get
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var json = await db.views.listAgronomicDataAsync();
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
