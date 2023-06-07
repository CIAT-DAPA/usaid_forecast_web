using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    [EnableCors("SiteCorsPolicy")]
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
        [Route("api/[controller]/{cultivar}/{format}")]
        public async Task<IActionResult> Get(bool cultivar, string format)
        {
            try
            {
                var json = await db.views.listAgronomicDataAsync();
                // Write event log
                writeEvent("Agronomic count [" + json.Count().ToString() + "]", LogEvent.lis);

                //Evaluate the format to export
                if (string.IsNullOrEmpty(format) || format.ToLower().Trim().Equals("json"))
                    return Json(json);
                else if (format.ToLower().Trim().Equals("csv"))
                {
                    StringBuilder builder = new StringBuilder();
                    // add header depends if the requested a cultivars information or soils
                    if (cultivar)
                        builder.Append(string.Join<string>(delimiter, new string[] { "crop_id", "crop_name", "cultivar_id", "cultivar_name", "cultivar_rainfed", "cultivar_national", "country_id", "\n" }));
                    else
                        builder.Append(string.Join<string>(delimiter, new string[] { "crop_id", "crop_name", "soil_id", "soil_name", "country_id", "\n" }));
                    foreach (var i in json)
                    {
                        if (cultivar)
                            foreach (var j in i.cultivars)
                                builder.Append(string.Join<string>(delimiter, new string[] { i.cp_id, i.cp_name, j.id, j.name, j.rainfed.ToString(), j.national.ToString(), j.country_id, "\n" }));
                        else
                            foreach (var j in i.soils)
                                builder.Append(string.Join<string>(delimiter, new string[] { i.cp_id, i.cp_name, j.id, j.name, j.country_id, "\n" }));
                    }
                    var file = UnicodeEncoding.Unicode.GetBytes(builder.ToString());
                    return File(file, "text/csv", "agronomic.csv");
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
