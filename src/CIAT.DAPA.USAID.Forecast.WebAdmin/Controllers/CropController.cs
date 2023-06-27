using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Extend;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
    [Authorize(Roles = "ADMIN,IMPROVER")]
    [Authorize]
    public class CropController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public CropController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) : 
            base(settings, LogEntity.cp_crop, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }

        // GET: /Crop/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.crop.listEnableAsync();
                await writeEventAsync(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
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
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Crop entity = await db.crop.byIdAsync(id);
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
                //entity.setup = new List<Setup>();
                if (ModelState.IsValid)
                {
                    await db.crop.insertAsync(entity);
                    await writeEventAsync(entity.ToString(), LogEvent.cre);
                    return RedirectToAction("Index");
                }
                await writeEventAsync(ModelState.ToString(), LogEvent.err);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
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
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Crop entity = await db.crop.byIdAsync(id);
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
                    await writeEventAsync(entity.ToString(), LogEvent.upd);
                    return RedirectToAction("Index");
                }
                await writeEventAsync(ModelState.ToString(), LogEvent.err);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
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
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Crop entity = await db.crop.byIdAsync(id);
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

        // POST: /Crop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Crop entity = await db.crop.byIdAsync(id);
                await db.crop.deleteAsync(entity);
                await writeEventAsync(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
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
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Crop entity = await db.crop.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                // Set data for the view
                ViewBag.crop_name = entity.name;
                ViewBag.crop_id = entity.id;
                // Get the data
                var ws = await db.weatherStation.listEnableVisibleAsync();
                var cu = await db.cultivar.listEnableAsync();
                var so = await db.soil.listEnableAsync();
                List<CropSetup> entities = new List<CropSetup>();
                //foreach (var c in entity.setup)
                //    entities.Add(new CropSetup(c, ws, cu, so));
                // Fill the select list
                ViewBag.weather_station = new SelectList(ws, "id", "name");
                ViewBag.cultivar = new SelectList(cu.Where(p => p.crop == entity.id), "id", "name");
                ViewBag.soil = new SelectList(so.Where(p => p.crop == entity.id), "id", "name");
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entities);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
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
                // Instance the new setup entity
                Setup setup = new Setup()
                {
                    weather_station = getId(form["weather_station"]),
                    cultivar = getId(form["cultivar"]),
                    soil = getId(form["soil"]),
                    days = int.Parse(form["days"])
                };
                // Saving the data of the configuration files
                List<ConfigurationFile> files = new List<ConfigurationFile>();
                ConfigurationFile file_temp;
                int i = 0;
                // This cicle add all files upload to the setup entity
                foreach (var f in form.Files)
                {
                    i += 1;
                    file_temp = new ConfigurationFile()
                    {
                        date = DateTime.Now,
                        path = configurationPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-setup-" + id + "-" + f.FileName,
                        name = form["name_f_" + i.ToString()]
                    };
                    // Save a copy of the file in the server
                    using (var stream = new FileStream(file_temp.path, FileMode.Create))
                    {
                        stream.Position = 0;
                        await f.CopyToAsync(stream);
                    }
                    files.Add(file_temp);
                }
                // Set the files to the setup entity
                setup.conf_files = files;
                //await db.crop.addSetupAsync(entity_new, setup);
                await writeEventAsync(id + " setup add: " + setup.weather_station.ToString() + "|" + setup.cultivar.ToString() + "|" + setup.soil.ToString() + "|" + setup.days.ToString(), LogEvent.upd);
                return RedirectToAction("Setup", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Setup");
            }
        }

        // POST: /Crop/SetupDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetupDelete(string crop, string ws, string cu, string so, int days)
        {
            try
            {
                // Get original crop data
                Crop entity_new = await db.crop.byIdAsync(crop);
                // Delete the setup
                //await db.crop.deleteSetupAsync(entity_new, ws, cu, so, days);
                await writeEventAsync(crop + " setup del: " + ws + "|" + cu + "|" + so + "|" + days.ToString(), LogEvent.upd);
                return RedirectToAction("Setup", new { id = crop });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Setup", new { id = crop });
            }
        }

        // GET: /Crop/CropConfig/5
        [HttpGet]
        public async Task<IActionResult> CropConfig(string id)
        {
            Crop entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.crop.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                // Set data for the view
                ViewBag.cp_name = entity.name;
                ViewBag.cp_id = entity.id;
                // Get data 

                List<CropConfig> entities = new List<CropConfig>();
                if (entity.crop_config != null)
                {
                    entities = (List<CropConfig>)entity.crop_config;
                }

                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entities);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /Crop/CropConfig/5
        [HttpPost, ActionName("CropConfig")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CropConfigAdd(string id)
        {
            try
            {
                // Get original weather station data
                var form = HttpContext.Request.Form;
                Crop entity_new = await db.crop.byIdAsync(id);
                // Instance the new range entity
                CropConfig crop_config = new CropConfig()
                {
                    label = form["label"],
                    min = double.Parse(form["min"]),
                    max = double.Parse(form["max"]),
                    type = form["type"]
                };
                await db.crop.addCropConfigAsync(entity_new, crop_config);
                await writeEventAsync(id + "Crop Configuration add: " + crop_config.label + "-" + crop_config.min.ToString() + "-" + crop_config.max.ToString() + "-" + crop_config.type.ToString(), LogEvent.upd);
                return RedirectToAction("CropConfig", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("CropConfig", new { id = id });
            }
        }

        // POST: /Crop/CropConfigDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CropConfigDelete(string crop, string label, double min, double max, string type)
        {
            try
            {
                // Get original crop data
                Crop entity_new = await db.crop.byIdAsync(crop);
                // Delete the setup
                await db.crop.deleteCropConfigAsync(entity_new, label, min, max, type);
                await writeEventAsync(crop + "Configuration del: " + label + "-" + min.ToString() + "-" + max.ToString(), LogEvent.upd);
                return RedirectToAction("CropConfig", new { id = crop });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("CropConfig", new { id = crop });
            }
        }
    }
}
