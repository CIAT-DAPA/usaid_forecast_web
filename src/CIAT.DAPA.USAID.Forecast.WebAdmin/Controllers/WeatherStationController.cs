using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Import;
using MongoDB.Bson;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Extend;
using CIAT.DAPA.USAID.Forecast.Data.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    //[Authorize(Roles = "ADMIN,CLIMATOLOGIST")]
    [Authorize]
    public class WeatherStationController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public WeatherStationController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) : 
            base(settings, LogEntity.lc_weather_station, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }

        // GET: /WeatherStation/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.weatherStation.listEnableAsync();
                ViewBag.municipalities = await db.municipality.listAllAsync();
                await writeEventAsync(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }

        }

        // GET: /WeatherStation/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // GET: /WeatherStation/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await generateListMunicipalitiesAsync(string.Empty);
            return View();
        }

        // POST: /WeatherStation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WeatherStation entity)
        {
            try
            {
                entity.municipality = getId(HttpContext.Request.Form["municipality"].ToString());
                if (ModelState.IsValid)
                {
                    await db.weatherStation.insertAsync(entity);
                    await writeEventAsync(entity.ToString(), LogEvent.cre);
                    return RedirectToAction("Index");
                }
                await writeEventAsync(ModelState.ToString(), LogEvent.err);
                await generateListMunicipalitiesAsync(string.Empty);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                await generateListMunicipalitiesAsync(string.Empty);
                return View(entity);
            }
        }

        // GET: /WeatherStation/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            WeatherStation entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                await generateListAllMunicipalitiesAsync(entity.municipality.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                await generateListAllMunicipalitiesAsync(entity.municipality.ToString());
                return View(entity);
            }
        }

        // POST: /WeatherStation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WeatherStation entity, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    WeatherStation current_entity = await db.weatherStation.byIdAsync(id);

                    entity.id = getId(id);
                    entity.municipality = getId(HttpContext.Request.Form["municipality"].ToString());
                    await db.weatherStation.updateAsync(current_entity, entity);
                    await writeEventAsync(entity.ToString(), LogEvent.upd);
                    return RedirectToAction("Index");
                }
                await writeEventAsync(ModelState.ToString(), LogEvent.err);
                await generateListAllMunicipalitiesAsync(entity.municipality.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                await generateListAllMunicipalitiesAsync(entity.municipality.ToString());
                return View(entity);
            }
        }

        // GET: /WeatherStation/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /WeatherStation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                await db.weatherStation.deleteAsync(entity);
                await writeEventAsync(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }                    

        // GET: /WeatherStation/Range/5
        [HttpGet]
        public async Task<IActionResult> Range(string id)
        {
            WeatherStation entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                // Set data for the view
                ViewBag.ws_name = entity.name;
                ViewBag.ws_id = entity.id;
                // Get data 
                var cp = await db.crop.listEnableAsync();
                // 
                List<CropYieldRange> entities = new List<CropYieldRange>();
                foreach (var r in entity.ranges)
                    entities.Add(new CropYieldRange(r, cp));
                // Fill the select list
                ViewBag.crop = new SelectList(cp, "id", "name");
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entities);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /WeatherStation/Range/5
        [HttpPost, ActionName("Range")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RangeAdd(string id)
        {
            try
            {
                // Get original weather station data
                var form = HttpContext.Request.Form;
                WeatherStation entity_new = await db.weatherStation.byIdAsync(id);
                // Instance the new range entity
                YieldRange range = new YieldRange()
                {
                    crop = getId(form["crop"]),
                    label = form["label"],
                    lower = int.Parse(form["lower"]),
                    upper = int.Parse(form["upper"])
                };
                await db.weatherStation.addRangeAsync(entity_new, range);
                await writeEventAsync(id + "range add: " + range.crop.ToString() + "-" + range.label + "-" + range.lower.ToString() + "-" + range.upper.ToString(), LogEvent.upd);
                return RedirectToAction("Range", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Range", new { id = id });
            }
        }

        // POST: /WeatherStation/RangeDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RangeDelete(string ws, string crop, string label, int lower, int upper)
        {
            try
            {
                // Get original crop data
                WeatherStation entity_new = await db.weatherStation.byIdAsync(ws);
                // Delete the setup
                await db.weatherStation.deleteRangeAsync(entity_new, crop, label, lower, upper);
                await writeEventAsync(ws + "range del: " + crop + "-" + label + "-" + lower.ToString() + "-" + upper.ToString(), LogEvent.upd);
                return RedirectToAction("Range", new { id = ws });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Range", new { id = ws });
            }
        }

        // GET: /WeatherStation/Range/5
        [HttpGet]
        public async Task<IActionResult> Configuration(string id)
        {
            WeatherStation entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                // Set data for the view
                ViewBag.ws_name = entity.name;
                ViewBag.ws_id = entity.id;
                // 
                var entities = entity.conf_files;
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entities);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /WeatherStation/Configuration/5
        [HttpPost, ActionName("Configuration")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurationAdd(string id)
        {
            try
            {
                // Get original weather station data
                var form = HttpContext.Request.Form;
                WeatherStation entity_new = await db.weatherStation.byIdAsync(id);
                // Instance the new range entity
                ConfigurationFile file_temp = new ConfigurationFile()
                {
                    date = DateTime.Now,
                    path = configurationPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-wsconf-" + id + "-" + form.Files[0].FileName,
                    name = form["name"]
                };
                // Save a copy of the file in the server
                using (var stream = new FileStream(file_temp.path, FileMode.Create))
                {
                    stream.Position = 0;
                    await form.Files[0].CopyToAsync(stream);
                }               

                await db.weatherStation.addConfigurationFileAsync(entity_new, file_temp);
                await writeEventAsync(id + "file add: " + entity_new.id.ToString() + "-" + form.Files[0].FileName, LogEvent.upd);
                return RedirectToAction("Configuration", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Configuration", new { id = id });
            }
        }
        /*
        /// <summary>
        /// Method to create a select list with the measure available to import
        /// </summary>
        private async Task generateListMeasuresAndSourceAsync()
        {
            // List climate variables
            var measures = from MeasureClimatic mc in Enum.GetValues(typeof(MeasureClimatic))
                           select new { id = (int)mc, name = mc.ToString() };
            ViewBag.measures = new SelectList(measures, "id", "name");
            // List sources
            var sources = await db.source.listEnableAsync();
            ViewBag.source = new SelectList(sources, "id", "name");
        }*/

        /// <summary>
        /// Method that create a select list with the municipalities available
        /// </summary>
        /// <param name="selected">The id of the entity, if it is empty or null, it will takes the first</param>
        private async Task<bool> generateListMunicipalitiesAsync(string selected)
        {
            var municipalities = (await db.municipality.listEnableAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            if (string.IsNullOrEmpty(selected))
                ViewData["municipality"] = new SelectList(municipalities, "id", "name");
            else
                ViewData["municipality"] = new SelectList(municipalities, "id", "name", selected);
            return municipalities.Count() > 0;
        }
        private async Task<bool> generateListAllMunicipalitiesAsync(string selected)
        {
            var municipalities = (await db.municipality.listAllAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            if (string.IsNullOrEmpty(selected))
                ViewData["municipality"] = new SelectList(municipalities, "id", "name");
            else
                ViewData["municipality"] = new SelectList(municipalities, "id", "name", selected);
            return municipalities.Count() > 0;
        }
    }
}
