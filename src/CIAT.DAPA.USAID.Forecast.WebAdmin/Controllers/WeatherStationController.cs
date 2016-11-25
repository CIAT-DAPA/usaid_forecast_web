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

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    public class WeatherStationController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        public WeatherStationController(IOptions<Settings> settings) : base(settings, LogEntity.lc_weather_station)
        {
        }

        // GET: /State/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.weatherStation.listEnableAsync();
                writeEvent(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }

        }

        // GET: /State/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // GET: /State/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var municipalities = await db.municipality.listEnableAsync();
            ViewBag.municipality = new SelectList(municipalities, "id", "name");
            return View();
        }

        // POST: /State/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WeatherStation entity)
        {
            try
            {
                entity.municipality = getId(HttpContext.Request.Form["municipality"].ToString());
                entity.conf_files = new List<ConfigurationFile>();
                entity.ranges = new List<YieldRange>();
                if (ModelState.IsValid)
                {
                    await db.weatherStation.insertAsync(entity);
                    writeEvent(entity.ToString(), LogEvent.cre);
                    return RedirectToAction("Index");
                }
                writeEvent(ModelState.ToString(), LogEvent.err);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View(entity);
            }
        }

        // GET: /State/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                var municipalities = await db.municipality.listEnableAsync();
                ViewBag.municipality = new SelectList(municipalities, "id", "name");
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // POST: /State/Edit/5
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
                    writeEvent(entity.ToString(), LogEvent.upd);
                    return RedirectToAction("Index");
                }
                writeEvent(ModelState.ToString(), LogEvent.err);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View(entity);
            }
        }

        // GET: /State/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // POST: /State/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                await db.weatherStation.deleteAsync(entity);
                writeEvent(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }
    }
}
