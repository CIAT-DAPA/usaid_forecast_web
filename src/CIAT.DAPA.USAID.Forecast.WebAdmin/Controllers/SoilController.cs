using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize(Roles = "ADMIN,IMPROVER")]
    [Authorize]
    public class SoilController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public SoilController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) : 
            base(settings, LogEntity.cp_soil, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }

        private async Task<PermissionList> LoadEnableByPermissionAsync(string countrySelected = "")
        {
            UserPermission permission = await getPermissionAsync();
            PermissionList val = new PermissionList();
            val.countries = await db.country.listEnableAsync();
            val.countries = val.countries.Where(p => permission.countries.Contains(p.id)).ToList();
            val.soils = await db.soil.listEnableAsync();
            if (countrySelected != "" && countrySelected != "000000" && permission.countries.Count() > 1)
            {
                val.soils = val.soils.Where(p => permission.countries.Contains(p.country) && p.country == new MongoDB.Bson.ObjectId(countrySelected)).ToList();
            }
            else if (countrySelected == "" && permission.countries.Count() > 1)
            {
                val.soils = val.soils.Where(p => permission.countries.Contains(p.country) && p.country == permission.countries.FirstOrDefault()).ToList();
            }
            else
            {
                val.soils = val.soils.Where(p => permission.countries.Contains(p.country)).ToList();
            }
            return val;
        }

        // GET: /Soil/
        [HttpGet]
        public async Task<IActionResult> Index(string countrySelect = "")
        {
            try
            {
                ViewBag.crops = await db.crop.listEnableAsync();
                var obj = await LoadEnableByPermissionAsync(countrySelect);
                var list = obj.soils;

                if (countrySelect == "")
                {
                    ViewBag.countrySelected = obj.countries[0].id.ToString();
                }
                else
                {
                    ViewBag.countrySelected = "";
                }
                ViewData["countriesData"] = getCountriesListWithDefult(obj, countrySelect);
                string dataJson = JsonConvert.SerializeObject(getListOfCountrySoils(obj));
                ViewBag.data = dataJson;

                await writeEventAsync(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }

        }

        // GET: /Soil/Details/5
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
                Soil entity = await db.soil.byIdAsync(id);
                ViewBag.country = await db.country.byIdAsync(entity.country.ToString());
                ViewBag.crop = await db.crop.byIdAsync(entity.crop.ToString());
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

        // GET: /Soil/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var crops = await db.crop.listEnableAsync();
            ViewBag.crop = new SelectList(crops, "id", "name");
            await generateListCountriesAsync(string.Empty);
            return View();
        }

        // POST: /Soil/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Soil entity)
        {
            try
            {
                entity.crop = getId(HttpContext.Request.Form["crop"].ToString());
                entity.country = getId(HttpContext.Request.Form["country"].ToString());
                if (ModelState.IsValid)
                {
                    await db.soil.insertAsync(entity);
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

        // GET: /Soil/Edit/5
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
                Soil entity = await db.soil.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                var crops = await db.crop.listEnableAsync();
                ViewBag.crop = new SelectList(crops, "id", "name", entity.crop);
                await generateListCountriesAsync(entity.country.ToString());
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /Soil/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Soil entity, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Soil current_entity = await db.soil.byIdAsync(id);

                    entity.id = getId(id);
                    entity.crop = getId(HttpContext.Request.Form["crop"].ToString());
                    entity.country = getId(HttpContext.Request.Form["country"].ToString());
                    await db.soil.updateAsync(current_entity, entity);
                    await writeEventAsync(entity.ToString(), LogEvent.upd);
                    return RedirectToAction("Index");
                }
                await writeEventAsync(ModelState.ToString(), LogEvent.err);
                return View(entity);
            }
            catch (Exception ex)
            {
                await generateListCountriesAsync(entity.country.ToString());
                await writeExceptionAsync(ex);
                return View(entity);
            }
        }

        // GET: /Soil/Delete/5
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
                Soil entity = await db.soil.byIdAsync(id);
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

        // POST: /Soil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Soil entity = await db.soil.byIdAsync(id);
                await db.soil.deleteAsync(entity);
                await writeEventAsync(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }

        // GET: /Soil/Threshold/5
        [HttpGet]
        public async Task<IActionResult> Threshold(string id)
        {
            Soil entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.soil.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                // Set data for the view
                ViewBag.soil_name = entity.name;
                ViewBag.soil_id = entity.id;
                // Get data 

                List<Threshold> entities = new List<Threshold>();
                if(entity.threshold != null)
                {
                    entities = (List<Threshold>)entity.threshold;
                }
                
                // Fill the select list
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entities);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /Soil/Threshold/5
        [HttpPost, ActionName("Threshold")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThresholdAdd(string id)
        {
            try
            {
                // Get original soil data
                var form = HttpContext.Request.Form;
                Soil entity_new = await db.soil.byIdAsync(id);
                // Instance the new threshold entity
                Threshold threshold = new Threshold()
                {
                    label = form["label"],
                    value = double.Parse(form["value"]),
                };
                await db.soil.addThresholdAsync(entity_new, threshold);
                await writeEventAsync(id + "Threshold add: " + entity_new.id.ToString() + "-" + threshold.label + "-" + threshold.value.ToString(), LogEvent.upd);
                return RedirectToAction("Threshold", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Threshold", new { id = id });
            }
        }

        // POST: /Soil/ThresholdDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThresholdDelete(string soil_id, string label, double value)
        {
            try
            {
                // Get original soil data
                Soil entity_new = await db.soil.byIdAsync(soil_id);
                // Delete the setup
                await db.soil.deleteThresholdAsync(entity_new, label, value);
                await writeEventAsync(soil_id + "Threshold del: " + label + "-" + value.ToString(), LogEvent.upd);
                return RedirectToAction("Threshold", new { id = soil_id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Threshold", new { id = soil_id });
            }
        }

        private async Task<bool> generateListCountriesAsync(string selected)
        {
            var obj = await LoadEnableByPermissionAsync();
            // Filter states by permission by countries            
            var countries = obj.countries.Select(p => new { id = p.id.ToString(), name = p.name });
            if (string.IsNullOrEmpty(selected))
                ViewData["country"] = new SelectList(countries, "id", "name");
            else
                ViewData["country"] = new SelectList(countries, "id", "name", selected);
            return countries.Count() > 0;
        }


        private List<Object> getListOfCountrySoils(PermissionList obj)
        {
            List<object> listCountryData = new List<object>();
            foreach (Country country in obj.countries)
            {
                IEnumerable<Soil> ct = obj.soils.Where(s => s.country == country.id);

                var countryData = new
                {
                    countryId = country.id,
                    listData = ct
                };
                listCountryData.Add(countryData);
            }
            return listCountryData;
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

        [HttpGet]
        public async Task<ActionResult> ObtainCountryFilterData()
        {
            var obj = await LoadEnableByPermissionAsync();
            string dataJson = JsonConvert.SerializeObject(getListOfCountrySoils(obj));
            return Content(dataJson, "application/json");
        }
    }
}
