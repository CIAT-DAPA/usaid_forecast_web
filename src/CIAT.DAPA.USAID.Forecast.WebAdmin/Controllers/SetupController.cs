using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Extend;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize]
    public class SetupController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public SetupController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) :
            base(settings, LogEntity.cp_setup, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }

        // GET: SetupController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var setups = await db.setup.listEnableAsync();
                // Get the data
                var crp = await db.crop.listEnableAsync();
                var ws = await db.weatherStation.listEnableVisibleAsync();
                var cu = await db.cultivar.listEnableAsync();
                var so = await db.soil.listEnableAsync();
                List<SetupExtend> entities = new List<SetupExtend>();
                foreach (var s in setups)
                {
                    entities.Add(new SetupExtend(s, crp, ws, cu, so));
                }
                await writeEventAsync(setups.Count().ToString(), LogEvent.lis);

                return View(entities);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // GET: SetupController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Setup entity = await db.setup.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                var crp = await db.crop.listEnableAsync();
                var ws = await db.weatherStation.listEnableVisibleAsync();
                var cu = await db.cultivar.listEnableAsync();
                var so = await db.soil.listEnableAsync();

                ViewBag.crop_name = crp.SingleOrDefault(p => p.id == entity.crop).name;
                ViewBag.ws_name = ws.SingleOrDefault(p => p.id == entity.weather_station).name;
                ViewBag.cultivar_name = cu.SingleOrDefault(p => p.id == entity.cultivar).name;
                ViewBag.soil_name = so.SingleOrDefault(p => p.id == entity.soil).name;

                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // GET: SetupController/Create
        public async Task<IActionResult> Create()
        {
            var setups = await db.setup.listEnableAsync();
            // Get the data
            var crp = await db.crop.listEnableAsync();
            var ws = await db.weatherStation.listEnableVisibleAsync();
            ViewBag.crop = new SelectList(crp, "id", "name");
            ViewBag.weather_station = new SelectList(ws, "id", "name");

            return View();
        }

        // POST: SetupController/Create
        [HttpPost, ActionName("CreateSetup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Setup entity)
        {
            try
            {
                var form = HttpContext.Request.Form;
                entity.crop = getId(form["crop"]);
                entity.weather_station = getId(form["weather_station"]);
                entity.cultivar = getId(form["cultivar"]);
                entity.soil = getId(form["soil"]);
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
                        path = configurationPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-setup-" + entity.crop + "-" + f.FileName,
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
                entity.conf_files = files;
                if (ModelState.IsValid)
                {
                    await db.setup.insertAsync(entity);
                    await writeEventAsync(entity.ToString(), LogEvent.cre);

                    return RedirectToAction("Index");
                }
                await writeEventAsync(ModelState.ToString(), LogEvent.err);

                return View();
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // GET: SetupController/Edit/5
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
                Setup entity = await db.setup.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                await generateListingsAsync(entity);

                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: SetupController/Edit/5
        [HttpPost, ActionName("EditSetup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Setup entity, string id)
        {
            try
            {
                var form = HttpContext.Request.Form;
                entity.crop = getId(form["crop"]);
                entity.weather_station = getId(form["weather_station"]);
                entity.cultivar = getId(form["cultivar"]);
                entity.soil = getId(form["soil"]);
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
                        path = configurationPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-setup-" + entity.crop + "-" + f.FileName,
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
                if (ModelState.IsValid)
                {
                    Setup current_entity = await db.setup.byIdAsync(id);
                    var oldFiles = current_entity.conf_files.ToList();
                    oldFiles.AddRange(files);
                    entity.conf_files = oldFiles;
                    entity.id = getId(id);
                    await db.setup.updateAsync(current_entity, entity);
                    await writeEventAsync(entity.ToString(), LogEvent.upd);

                    return RedirectToAction("Index");
                }
                await generateListingsAsync(entity);

                return View();
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                await generateListingsAsync(entity);

                return View();
            }
        }

        // GET: SetupController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Setup entity = await db.setup.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                var crp = await db.crop.listEnableAsync();
                var ws = await db.weatherStation.listEnableVisibleAsync();
                var cu = await db.cultivar.listEnableAsync();
                var so = await db.soil.listEnableAsync();

                ViewBag.crop_name = crp.SingleOrDefault(p => p.id == entity.crop).name;
                ViewBag.ws_name = ws.SingleOrDefault(p => p.id == entity.weather_station).name;
                ViewBag.cultivar_name = cu.SingleOrDefault(p => p.id == entity.cultivar).name;
                ViewBag.soil_name = so.SingleOrDefault(p => p.id == entity.soil).name;

                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: SetupController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Setup entity = await db.setup.byIdAsync(id);
                await db.setup.deleteAsync(entity);
                await writeEventAsync(entity.ToString(), LogEvent.del);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }

        public async Task<JsonResult> GetCultivarsByIdCrop(string cropId)
        {
            List<Cultivar> cultivarList = await db.cultivar.listEnableAsync();
            var cuListCondition = cultivarList.Where(p => p.crop.ToString() == cropId);

            return Json(cuListCondition);
        }

        public async Task<JsonResult> GetSoilsByIdCrop(string cropId)
        {
            List<Soil> soilList = await db.soil.listEnableAsync();
            var soListCondition = soilList.Where(p => p.crop.ToString() == cropId);

            return Json(soListCondition);
        }
        private async Task<bool> generateListingsAsync(Setup entity)
        {
            var crp = (await db.crop.listEnableAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            ViewData["crop"] = new SelectList(crp, "id", "name", entity.crop.ToString());
            var ws = (await db.weatherStation.listEnableVisibleAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            ViewData["ws"] = new SelectList(ws, "id", "name", entity.weather_station.ToString());
            var cu = (await db.cultivar.listEnableAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            ViewData["cultivar"] = new SelectList(cu, "id", "name", entity.cultivar.ToString());
            var so = (await db.soil.listEnableAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            ViewData["soil"] = new SelectList(so, "id", "name", entity.soil.ToString());

            return true;
        }

        public async Task<bool> deleteFileConf(int idConf, string idSetup)
        {
            try
            {
                if (string.IsNullOrEmpty(idSetup))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return false;
                }
                Setup entity = await db.setup.byIdAsync(idSetup);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + idSetup, LogEvent.err);
                    return false;
                }
                var files = entity.conf_files.ToList();
                files.RemoveAt(idConf);
                entity.conf_files = files;
                var bolean = await db.setup.updateAsync(entity, entity);
                return bolean;
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);

                return false;
            }
        }
    }
}
