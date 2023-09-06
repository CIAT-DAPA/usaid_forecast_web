using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize(Roles = "ADMIN,CLIMATOLOGIST")]
    [Authorize]
    public class StateController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public StateController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) :
            base(settings, LogEntity.lc_state, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }

        private async Task<PermissionList> LoadEnableByPermissionAsync(string countrySelected = "")
        {
            UserPermission permission = await getPermissionAsync();
            PermissionList val = new PermissionList();
            val.countries = await db.country.listEnableAsync();
            val.countries = val.countries.Where(p => permission.countries.Contains(p.id)).ToList();
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
            return val;
        }

        private async Task<PermissionList> LoadAllByPermissionAsync()
        {
            UserPermission permission = await getPermissionAsync();
            PermissionList val = new PermissionList();
            val.countries = await db.country.listAllAsync();
            val.countries = val.countries.Where(p => permission.countries.Contains(p.id)).ToList();
            val.states = await db.state.listAllAsync();
            val.states = val.states.Where(p => permission.countries.Contains(p.country)).ToList();
            return val;
        }

        // GET: /State/
        [HttpGet]
        public async Task<IActionResult> Index(string countrySelect = "")
        {
            try
            {
                var obj = await LoadEnableByPermissionAsync(countrySelect);
                var list = obj.states;                
                ViewBag.countries = obj.countries;

                if (countrySelect == "")
                {
                    ViewBag.countrySelected = obj.countries[0].id.ToString();
                }
                else
                {
                    ViewBag.countrySelected = "";
                }
                ViewData["countriesData"] = getCountriesListWithDefult(obj, countrySelect);
                string dataJson = JsonConvert.SerializeObject(getListOfCountryStates(obj));
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

        // GET: /State/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                ViewBag.countries = await db.country.listAllAsync();
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                State entity = await db.state.byIdAsync(id);
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

        // GET: /State/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await generateListCountriesAsync(string.Empty);
            return View();
        }

        // POST: /State/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(State entity)
        {
            try
            {
                entity.country = getId(HttpContext.Request.Form["country"].ToString());
                if (ModelState.IsValid)
                {
                    await db.state.insertAsync(entity);
                    await writeEventAsync(entity.ToString(), LogEvent.cre);
                    return RedirectToAction("Index");
                }
                await writeEventAsync(ModelState.ToString(), LogEvent.err);
                await generateListCountriesAsync(string.Empty);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                await generateListCountriesAsync(string.Empty);
                return View(entity);
            }
        }

        // GET: /State/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            State entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.state.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                await generateListAllCountriesAsync(entity.country.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                await generateListAllCountriesAsync(entity.country.ToString());
                return View();
            }
        }

        // POST: /State/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(State entity, string id)
        {
            try
            {
                entity.country = getId(HttpContext.Request.Form["country"].ToString());
                if (ModelState.IsValid)
                {
                    State current_entity = await db.state.byIdAsync(id);

                    entity.id = getId(id);
                    await db.state.updateAsync(current_entity, entity);
                    await writeEventAsync(entity.ToString(), LogEvent.upd);
                    return RedirectToAction("Index");
                }
                await writeEventAsync(ModelState.ToString(), LogEvent.err);
                await generateListAllCountriesAsync(entity.country.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                UserPermission permission = await getPermissionAsync();
                await generateListAllCountriesAsync(entity.country.ToString());
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
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                State entity = await db.state.byIdAsync(id);
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

        // POST: /State/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                State entity = await db.state.byIdAsync(id);
                await db.state.deleteAsync(entity);
                await writeEventAsync(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }

        // GET: /State/Import/5
        [HttpGet]
        public async Task<IActionResult> Import(string id)
        {
            try
            {
                ViewBag.countries = await db.country.listAllAsync();
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                State entity = await db.state.byIdAsync(id);
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

        // POST: /State/ImportMunicipalitiesWeatherStations
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportMunicipalitiesWeatherStations(string id, IFormFile file)
        {
            Message msg = null;
            State entity = null;
            try
            {
                ObjectId state_id = getId(id);
                entity = await db.state.byIdAsync(id);
                if (file != null && file.Length > 0)
                {
                    // Save a copy in the web site
                    await file.CopyToAsync(new FileStream(importPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-state-mws-" + file.FileName, FileMode.Create));
                    // Read the file
                    StreamReader reader = new StreamReader(file.OpenReadStream());
                    // Variables to process the file
                    IEnumerable<string> m_name = null, ws_ext_id = null, ws_name = null, ws_lat = null, ws_lon = null;
                    string ws_origin = string.Empty;
                    string line = string.Empty;
                    // Read the file
                    while (!reader.EndOfStream)
                    {
                        line = await reader.ReadLineAsync();
                        // read the headers
                        if (line.StartsWith("ext_id"))
                            ws_ext_id = line.Split(',').Skip(1);
                        else if (line.StartsWith("municipality"))
                            m_name = line.Split(',').Skip(1);
                        else if (line.StartsWith("name"))
                            ws_name = line.Split(',').Skip(1);
                        else if (line.StartsWith("latitude"))
                            ws_lat = line.Split(',').Skip(1);
                        else if (line.StartsWith("longitude"))
                            ws_lon = line.Split(',').Skip(1);
                        else if (line.StartsWith("origin"))
                            ws_origin = line.Split(',')[1];
                    }
                    // Variables to management the import process
                    int count_municipalities = 0, count_weather_stations = 0;
                    Municipality m_temp;
                    // Create all municipalities
                    foreach (string m in m_name)
                    {
                        m_temp = await db.municipality.byNameAsync(m);
                        if (m_temp == null)
                        {
                            await db.municipality.insertAsync(new Municipality() { name = m, state = state_id, visible = false });
                            count_municipalities += 1;
                        }
                    }

                    // Create all weather stations                    
                    for (int i = 0; i < ws_name.Count(); i++)
                    {
                        m_temp = await db.municipality.byNameAsync(m_name.ElementAt(i));
                        await db.weatherStation.insertAsync(new WeatherStation()
                        {
                            ext_id = ws_ext_id.ElementAt(i),
                            latitude = double.Parse(ws_lat.ElementAt(i), CultureInfo.InvariantCulture),
                            longitude = double.Parse(ws_lon.ElementAt(i), CultureInfo.InvariantCulture),
                            municipality = m_temp.id,
                            name = ws_name.ElementAt(i).Trim(),
                            origin = ws_origin,
                            visible = false
                        });
                        count_weather_stations += 1;
                    }
                    msg = new Message()
                    {
                        content = "Import MWS. The file was imported correctly. Records imported: (" +
                            count_municipalities.ToString() + ") municipalities (" + count_weather_stations.ToString() + ") weather stations",
                        type = MessageType.successful
                    };
                    await writeEventAsync(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_state, LogEntity.lc_municipality, LogEntity.lc_weather_station });
                }
                else
                {
                    msg = new Message() { content = "Import MWS. An error occurred with the file imported", type = MessageType.error };
                    await writeEventAsync(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_state, LogEntity.lc_municipality, LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {

                await writeExceptionAsync(ex);
                msg = new Message() { content = "Import MWS. An error occurred in the system, contact the administrator", type = MessageType.error };
            }
            ViewBag.message = msg;
            ViewBag.countries = await db.country.listAllAsync();
            return View("Import", entity );
        }

        // GET: /State/Configuration/5
        [HttpGet]
        public async Task<IActionResult> Configuration(string id)
        {
            try
            {
                ViewBag.countries = await db.country.listAllAsync();

                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                State entity = await db.state.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                ViewBag.state = entity;
                await generateListOfEnumsAsync();
                return View(entity.conf);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /State/Configuration/5
        [HttpPost, ActionName("Configuration")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurationAdd(string id)
        {
            try
            {
                // Get original state data
                var form = HttpContext.Request.Form;
                State entity_new = await db.state.byIdAsync(id);
                // Instance the new configuration entity

                ForecastType forc_type = (ForecastType)int.Parse(form["forc_type"]);

                ConfigurationCPT conf = new ConfigurationCPT()
                {
                    cca_mode = int.Parse(form["cca"]),
                    gamma = !form["gamma"].Equals("false"),
                    trimester = forc_type.ToString() == "tri" ? (Quarter)int.Parse(form["trimester"]) : (Quarter)int.Parse(form["bimonthly"]),
                    x_mode = int.Parse(form["x"]),
                    y_mode = int.Parse(form["y"]),
                    forc_type = forc_type,
                    predictand = (MeasureClimatic)int.Parse(form["measure"]),


                };
                int count = form.Keys.Where(p => p.Contains("left_") && p.Contains("_lat")).Count();
                List<Region> regions = new List<Region>();
                // This cicle add all regions
                for (int i = 1; i <= count; i++)
                {
                    regions.Add(new Region()
                    {
                        predictor = (ForecastPredictors)int.Parse(form["predictor_" + i.ToString()]),
                        left_lower = new Coords()
                        {
                            lat = double.Parse(form["left_lower_" + i.ToString() + "_lat"]),
                            lon = double.Parse(form["left_lower_" + i.ToString() + "_lon"])
                        },
                        rigth_upper = new Coords()
                        {
                            lat = double.Parse(form["right_upper_" + i.ToString() + "_lat"]),
                            lon = double.Parse(form["right_upper_" + i.ToString() + "_lon"])
                        }
                    });
                }
                if (regions.Count() == 0)
                    return RedirectToAction("Configuration", new { id = id });

                conf.regions = regions.ToList();
                await db.state.addConfigurationCPTAsync(entity_new, conf);
                await writeEventAsync(id + " conf add: " + conf.ToString(), LogEvent.upd);
                return RedirectToAction("Configuration", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Configuration", new { id = id });
            }
        }

        // POST: /State/ConfigurationDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurationDelete(string id, string quarter, int cca, bool gamma, int x, int y, double left_lat, double left_lon, double right_lat, double right_lon, DateTime register, string type, string predictand)
        {
            try
            {
                // Get original crop data
                State entity_new = await db.state.byIdAsync(id);
                // Delete the setup
                await db.state.deleteConfigurationCPTAsync(entity_new, (Quarter)Enum.Parse(typeof(Quarter), quarter), cca, gamma, x, y, register, (ForecastType)Enum.Parse(typeof(ForecastType), type), (MeasureClimatic)Enum.Parse(typeof(MeasureClimatic), predictand));
                await writeEventAsync(id + " conf del: " + quarter.ToString() + "|" + type.ToString() + predictand.ToString() + "|" + cca.ToString() + "|" + gamma.ToString() + "|" + x.ToString() + "|" + y.ToString() + "|" + left_lat.ToString() + "," + left_lon.ToString() + "|" + right_lat.ToString() + "," + right_lon, LogEvent.upd);
                return RedirectToAction("Configuration", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Configuration", new { id = id });
            }
        }

        private async Task generateListOfEnumsAsync()
        {
            // List climate variables
            var quarters = Enum.GetValues(typeof(Quarter))
                           .Cast<Quarter>()
                           .Where(q => q.ToString().Length > 2)
                           .Select(q => new { id = (int)q, name = q.ToString() });

            ViewBag.trimester = new SelectList(quarters, "id", "name");

            var bimonthly = Enum.GetValues(typeof(Quarter))
                           .Cast<Quarter>()
                           .Where(q => q.ToString().Length <= 2)
                           .Select(q => new { id = (int)q, name = q.ToString() });

            ViewBag.bimonthly = new SelectList(bimonthly, "id", "name");

            var type = from ForecastType q in Enum.GetValues(typeof(ForecastType))
                           select new { id = (int)q, name = q.ToString() };
            ViewBag.forc_type = new SelectList(type, "id", "name");


            var measures = from MeasureClimatic q in Enum.GetValues(typeof(MeasureClimatic))
                            select new { id = (int)q, name = q.ToString() };
            ViewBag.measure = new SelectList(measures, "id", "name");
        }


        /// <summary>
        /// Method that create a select list with the countries available
        /// </summary>
        /// <param name="selected">The id of the entity, if it is empty or null, it will takes the first</param>
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
        private async Task<bool> generateListAllCountriesAsync(string selected)
        {
            var obj = await LoadAllByPermissionAsync();            
            var countries = obj.countries.Select(p => new { id = p.id.ToString(), name = p.name });
            if (string.IsNullOrEmpty(selected))
                ViewData["country"] = new SelectList(countries, "id", "name");
            else
                ViewData["country"] = new SelectList(countries, "id", "name", selected);
            return countries.Count() > 0;
        }
        // GET: /Country/ConfigurationPyCpt/5
        [HttpGet]
        public async Task<IActionResult> ConfigurationPyCpt(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                State entity = await db.state.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                generateListsPyCPT();
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }
        
        // POST: /Satate/ConfigurationPyCpt/5
        [HttpPost, ActionName("ConfigurationPyCpt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurationPyCptAdd(string id)
        {
            try
            {
                var form = HttpContext.Request.Form;
                State entity = await db.state.byIdAsync(id);
                ConfigurationPyCPT confPyCpt = generatePyCPTConfAsync(form);
                await db.state.addConfigurationPyCpt(entity, confPyCpt);
                return RedirectToAction("ConfigurationPyCpt", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("ConfigurationPyCpt", new { id = id });
            }
        }
        // POST: /Country/ConfigurationDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurationPyCptDelete(string id, int month, long register)
        {   
            try
            {
                // Get original crop data
                State entity_new = await db.state.byIdAsync(id);
                // Delete the setup
                await db.state.deleteConfigurationPyCPTAsync(entity_new, month, register);
                return RedirectToAction("ConfigurationPyCpt", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("ConfigurationPyCpt", new { id = id });
            }
        }


        private List<Object> getListOfCountryStates(PermissionList obj)
        {
            List<object> listCountryData = new List<object>();
            foreach (Country country in obj.countries)
            {
                IEnumerable<State> st = obj.states.Where(s => s.country == country.id);
                var countryData = new
                {
                    countryId = country.id,
                    listData = st
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
            string dataJson = JsonConvert.SerializeObject(getListOfCountryStates(obj));
            return Content(dataJson, "application/json");
        }


        [HttpGet]
        public async Task<ActionResult> ObtainPredictorData()
        {
            var predictor = from ForecastPredictors q in Enum.GetValues(typeof(ForecastPredictors))
                            select new { id = (int)q, name = q.ToString() };
            string dataJson = JsonConvert.SerializeObject(predictor);
            return Content(dataJson, "application/json");
        }
    }
}
