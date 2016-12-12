﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
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
                var json = await db.views.listLocationVisibleAsync();
                writeEvent(json.Count().ToString(), LogEvent.lis);
                return Json(json);
            }
            catch(Exception ex)
            {
                writeException(ex);
                return new StatusCodeResult(500);
            }
        }
    }
}