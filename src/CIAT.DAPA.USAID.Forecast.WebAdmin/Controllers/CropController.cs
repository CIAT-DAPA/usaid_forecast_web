using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Extend;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    public class CropController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public CropController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, LogEntity.cp_crop, hostingEnvironment)
        {
        }

        // GET: /Crop/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.crop.listEnableAsync();
                writeEvent(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }

        }

        // GET: /Crop/Details/5
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
                Crop entity = await db.crop.byIdAsync(id);
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

        // GET: /Crop/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: /Crop/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Crop entity)
        {
            try
            {
                entity.setup = new List<Setup>();
                if (ModelState.IsValid)
                {
                    await db.crop.insertAsync(entity);
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

        // GET: /Crop/Edit/5
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
                Crop entity = await db.crop.byIdAsync(id);
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

        // POST: /Crop/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Crop entity, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Crop current_entity = await db.crop.byIdAsync(id);

                    entity.id = getId(id);
                    await db.crop.updateAsync(current_entity, entity);
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

        // GET: /Crop/Delete/5
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
                Crop entity = await db.crop.byIdAsync(id);
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

        // POST: /Crop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Crop entity = await db.crop.byIdAsync(id);
                await db.crop.deleteAsync(entity);
                writeEvent(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }

        // GET: /Crop/Setup/5
        [HttpGet]
        public async Task<IActionResult> Setup(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Crop entity = await db.crop.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                // Set data for the view
                ViewBag.crop_name = entity.name;
                ViewBag.crop_id = entity.id;
                // 
                var ws = await db.weatherStation.listEnableVisibleAsync();
                var cu = await db.cultivar.listEnableAsync();
                var so = await db.soil.listEnableAsync();
                List<CropSetup> entities = new List<CropSetup>();
                foreach (var c in entity.setup)
                    entities.Add(new CropSetup(c, ws, cu, so));
                //                
                ViewBag.weather_station = new SelectList(ws, "id", "name");
                ViewBag.cultivar = new SelectList(cu.Where(p => p.crop == entity.id), "id", "name");
                ViewBag.soil = new SelectList(so.Where(p => p.crop == entity.id), "id", "name");
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entities);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // POST: /Crop/Setup/5
        [HttpPost, ActionName("Setup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetupAdd(string id)
        {
            try
            {
                // Get original crop data
                var form = HttpContext.Request.Form;
                Crop entity_new = await db.crop.byIdAsync(id);
                // Instance
                Setup setup = new Setup()
                {
                    weather_station = getId(form["weather_station"]),
                    cultivar = getId(form["cultivar"]),
                    soil = getId(form["soil"]),
                    days = int.Parse(form["days"]),
                    track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now }
                };
                // Saving the data of the configuration files
                List<ConfigurationFile> files = new List<ConfigurationFile>();
                ConfigurationFile file_temp;
                int i = 0;
                foreach (var f in form.Files)
                {
                    i += 1;
                    // Save a copy in the web site
                    file_temp = new ConfigurationFile()
                    {
                        date = DateTime.Now,
                        path = configurationPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + id + "-" + f.FileName,
                        name = form["name_f_" + i.ToString()]
                    };
                    await f.CopyToAsync(new FileStream(file_temp.path, FileMode.Create));
                    files.Add(file_temp);
                }
                setup.conf_files = files;
                List<Setup> allSetups = entity_new.setup.ToList();
                allSetups.Add(setup);
                entity_new.setup = allSetups;
                await db.crop.updateSetupAsync(entity_new);
                writeEvent(id + "setup: " + entity_new.setup.Count().ToString(), LogEvent.upd);
                return RedirectToAction("Setup", new { id = id });
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Setup");
            }
        }
    }
}
