using X.PagedList;
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
using Newtonsoft.Json;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize(Roles = "ADMIN,IMPROVER")]
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

        private async Task<PermissionList> LoadEnableByPermission(string countrySelected = "")
        {
            UserPermission permission = await getPermissionAsync();
            PermissionList val = new PermissionList();
            var countries = await db.country.listEnableAsync();
            val.countries = countries.Where(p => permission.countries.Contains(p.id)).ToList();
            val.states = await db.state.listEnableAsync();
            if (countrySelected != "" && countrySelected != "000000" && permission.countries.Count() > 1)
            {
                val.states = val.states.Where(p => permission.countries.Contains(p.country) && p.country == new MongoDB.Bson.ObjectId(countrySelected)).ToList();
            }
            else if (countrySelected == "" && permission.countries.Count() > 1)
            {
                val.states = val.states.Where(p => permission.countries.Contains(p.country) && p.country == permission.countries.FirstOrDefault()).ToList();
            }
            else
            {
                val.states = val.states.Where(p => permission.countries.Contains(p.country)).ToList();
            }


            var states_ids = val.states.Select(p => p.id).ToList();
            val.municipalities = await db.municipality.listEnableAsync();
            val.municipalities = val.municipalities.Where(p => states_ids.Contains(p.state)).ToList();
            var municipalities_ids = val.municipalities.Select(p => p.id).ToList();
            val.weather_stations = await db.weatherStation.listEnableAsync();
            val.weather_stations = val.weather_stations.Where(p => municipalities_ids.Contains(p.municipality)).ToList();

            var weather_station_ids = val.weather_stations.Select(p => p.id).ToList();
            val.setups = await db.setup.listEnableAsync();
            val.setups = val.setups.Where(p => weather_station_ids.Contains(p.weather_station)).ToList();
            return val;
        }

        // GET: SetupController
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, int?page, string countrySelect = "")
        {
            try
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.DatesortParam = sortOrder == "Date" ? "date_asc" : "Date";
                ViewBag.CropsortParam = sortOrder == "Crop" ? "crop_desc" : "Crop";
                ViewBag.WSsortParam = sortOrder == "WS" ? "ws_desc" : "WS";
                ViewBag.CultivarsortParam = sortOrder == "Cultivar" ? "cultivar_desc" : "Cultivar";
                ViewBag.SoilsortParam = sortOrder == "Soil" ? "soil_desc" : "Soil";
                ViewBag.DayssortParam = sortOrder == "Days" ? "days_desc" : "Days";
                //var setups = await db.setup.listEnableDescAsync();
                var data_with_permissions = await LoadEnableByPermission(countrySelect);
                var setups = data_with_permissions.setups;
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
                switch (sortOrder) 
                {
                    case "date_asc":
                        entities = entities.OrderBy(p => p.track.register).ToList();
                        break;
                    case "Crop":
                        entities = entities.OrderBy(p => p.crop_name).ToList();
                        break;
                    case "crop_desc":
                        entities = entities.OrderByDescending(p => p.crop_name).ToList();
                        break;
                    case "WS":
                        entities = entities.OrderBy(p => p.weather_station_name).ToList();
                        break;
                    case "ws_desc":
                        entities = entities.OrderByDescending(p => p.weather_station_name).ToList();
                        break;
                    case "Cultivar":
                        entities = entities.OrderBy(p => p.cultivar_name).ToList();
                        break;
                    case "cultivar_desc":
                        entities = entities.OrderByDescending(p => p.cultivar_name).ToList();
                        break;
                    case "Soil":
                        entities = entities.OrderBy(p => p.soil_name).ToList();
                        break;
                    case "soil_desc":
                        entities = entities.OrderByDescending(p => p.soil_name).ToList();
                        break;
                    case "Days":
                        entities = entities.OrderBy(p => p.days).ToList();
                        break;
                    case "days_desc":
                        entities = entities.OrderByDescending(p => p.days).ToList();
                        break;
                    default:
                        Console.WriteLine(entities);
                        break;
                }
                var pageSize = 10;
                var pageNumber = (page ?? 1);

                if (countrySelect == "")
                {
                    ViewBag.countrySelected = data_with_permissions.countries[0].id.ToString();
                }
                else
                {
                    ViewBag.countrySelected = "";
                }
                ViewData["countriesData"] = getCountriesListWithDefult(data_with_permissions, countrySelect);

                return View(entities.ToPagedList(pageNumber, pageSize));
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

        private SelectList getCountriesListWithDefult(PermissionList obj, string selectedCountryId)
        {

            List<SelectListItem> originalList = new List<SelectListItem>(obj.countries.Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.name }));

            originalList.Insert(0, new SelectListItem { Value = "000000", Text = "------", Selected = false });

            if (string.IsNullOrEmpty(selectedCountryId) && originalList.Count > 0)
            {
                SelectListItem selectedItem = originalList[1];
                selectedItem.Selected = true;
            }
            else
            {
                SelectListItem selectedItem = originalList.FirstOrDefault(item => item.Value == selectedCountryId);
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;
                }
            }
            SelectList selectList = new SelectList(originalList, "Value", "Text");


            return selectList;
        }

    }
}
