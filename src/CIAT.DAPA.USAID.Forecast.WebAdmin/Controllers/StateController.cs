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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    //[Authorize(Roles = "ADMIN,CLIMATOLOGIST")]
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

        // GET: /State/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.state.listEnableAsync();
                ViewBag.countries = await db.country.listAllAsync();
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
            return View("Import", entity);
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
                await generateListQuarterAsync();
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
                ConfigurationCPT conf = new ConfigurationCPT()
                {
                    cca_mode = int.Parse(form["cca"]),
                    gamma = !form["gamma"].Equals("false"),
                    trimester = (Quarter)int.Parse(form["trimester"]),
                    x_mode = int.Parse(form["x"]),
                    y_mode = int.Parse(form["y"])

                };
                int count = form.Keys.Where(p => p.Contains("left_") && p.Contains("_lat")).Count();
                List<Region> regions = new List<Region>();
                // This cicle add all regions
                for (int i = 1; i <= count; i++)
                {
                    regions.Add(new Region()
                    {
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
        public async Task<IActionResult> ConfigurationDelete(string id, string quarter, int cca, bool gamma, int x, int y, double left_lat, double left_lon, double right_lat, double right_lon, DateTime register)
        {
            try
            {
                // Get original crop data
                State entity_new = await db.state.byIdAsync(id);
                // Delete the setup
                await db.state.deleteConfigurationCPTAsync(entity_new, (Quarter)Enum.Parse(typeof(Quarter), quarter), cca, gamma, x, y, register);
                await writeEventAsync(id + " conf del: " + quarter.ToString() + "|" + cca.ToString() + "|" + gamma.ToString() + "|" + x.ToString() + "|" + y.ToString() + "|" + left_lat.ToString() + "," + left_lon.ToString() + "|" + right_lat.ToString() + "," + right_lon, LogEvent.upd);
                return RedirectToAction("Configuration", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Configuration", new { id = id });
            }
        }

        private async Task generateListQuarterAsync()
        {
            // List climate variables
            var quarters = from Quarter q in Enum.GetValues(typeof(Quarter))
                           select new { id = (int)q, name = q.ToString() };
            ViewBag.trimester = new SelectList(quarters, "id", "name");
        }

        /// <summary>
        /// Method that create a select list with the countries available
        /// </summary>
        /// <param name="selected">The id of the entity, if it is empty or null, it will takes the first</param>
        private async Task<bool> generateListCountriesAsync(string selected)
        {
            var countries = (await db.country.listEnableAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            if (string.IsNullOrEmpty(selected))
                ViewData["country"] = new SelectList(countries, "id", "name");
            else
                ViewData["country"] = new SelectList(countries, "id", "name", selected);
            return countries.Count() > 0;
        }
        private async Task<bool> generateListAllCountriesAsync(string selected)
        {
            var countries = (await db.country.listAllAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
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
                ViewBag.state = entity;
                IEnumerable<dynamic> buleans = new List<dynamic> { new { id = 0, name = "True" }, new { id = 1, name = "False" } };
                ViewBag.station = new SelectList(buleans, "name", "name");
                ViewBag.force_download = new SelectList(buleans, "name", "name");
                ViewBag.single_models = new SelectList(buleans, "name", "name");
                ViewBag.forecast_anomaly = new SelectList(buleans, "name", "name");
                ViewBag.forecast_spi = new SelectList(buleans, "name", "name");
                await generateListModelsPyCpyAsync();
                await generateListObsAsync();
                await generateListMosAsync();
                await generateListPredictorsAsync();
                await generateListPredictandAsync();
                await generateListMonsAsync();
                return View(entity.conf_pycpt);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }
        private async Task generateListModelsPyCpyAsync()
        {
            List<SelectListItem> checks = new List<SelectListItem>();
            // List climate variables
            var modelspycpt = from ModelsPyCpt q in Enum.GetValues(typeof(ModelsPyCpt))
                              select new { id = (int)q, name = q.ToString() };
            foreach (var item in modelspycpt)
            {
                checks.Add(new SelectListItem { Text = item.name.ToString(), Value = item.id.ToString() });
            }
            ViewBag.modelspycpt = checks;
        }
        private async Task generateListObsAsync()
        {
            // List climate variables
            var obs = from Obs q in Enum.GetValues(typeof(Obs))
                      select new { id = (int)q, name = q.ToString() };
            ViewBag.obs = new SelectList(obs, "id", "name");
        }
        private async Task generateListMosAsync()
        {
            // List climate variables
            var mos = from Mos q in Enum.GetValues(typeof(Mos))
                      select new { id = (int)q, name = q.ToString() };
            ViewBag.mos = new SelectList(mos, "id", "name");
        }
        private async Task generateListPredictorsAsync()
        {
            // List climate variables
            var predictors = from Predictors q in Enum.GetValues(typeof(Predictors))
                             select new { id = (int)q, name = q.ToString() };
            ViewBag.predictors = new SelectList(predictors, "id", "name");
        }
        private async Task generateListPredictandAsync()
        {
            // List climate variables
            var predictand = from Predictand q in Enum.GetValues(typeof(Predictand))
                             select new { id = (int)q, name = q.ToString() };
            ViewBag.predictand = new SelectList(predictand, "id", "name");
        }
        private async Task generateListMonsAsync()
        {
            List<SelectListItem> checks2 = new List<SelectListItem>();
            // List climate variables
            var mons = from Mons q in Enum.GetValues(typeof(Mons))
                       select new { id = (int)q, name = q.ToString() };
            foreach (var item in mons)
            {
                checks2.Add(new SelectListItem { Text = item.name.ToString(), Value = item.id.ToString() });
            }
            ViewBag.mons = checks2;
            ViewBag.tgts = checks2;
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
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                List<Mons> arrtgts = new List<Mons>();
                foreach (var item in form["tgts"])
                {
                    if (item != "false")
                    {
                        arrtgts.Add((Mons)int.Parse(item));
                    }
                }
                List<Mons> arrmons = new List<Mons>();
                foreach (var item in form["mons"])
                {
                    if (item != "false")
                    {
                        arrmons.Add((Mons)int.Parse(item));
                    }
                }
                List<ModelsPyCpt> arrmodpycpt = new List<ModelsPyCpt>();
                foreach (var item in form["modelspycpt"])
                {
                    if (item != "false")
                    {
                        arrmodpycpt.Add((ModelsPyCpt)int.Parse(item));
                    }
                }
                int counttgtii = form.Keys.Where(p => p.Contains("tgtii_value_")).Count();
                List<String> arrtgtii = new List<string>();
                for (int i = 0; i < counttgtii; i++)
                {
                    int f = i + 1;
                    arrtgtii.Add(form["tgtii_value_" + f.ToString()]);
                }
                int counttgtff = form.Keys.Where(p => p.Contains("tgtff_value_")).Count();
                List<String> arrtgtff = new List<string>();
                for (int i = 0; i < counttgtff; i++)
                {
                    int f = i + 1;
                    arrtgtff.Add(form["tgtff_value_" + f.ToString()]);
                }
                SpatialCoords spt_predictors = new SpatialCoords()
                {
                    nla = int.Parse(form["northernmost_lat1"]),
                    sla = int.Parse(form["southernmost_lat1"]),
                    wlo = int.Parse(form["westernmost_lat1"]),
                    elo = int.Parse(form["easternmost_lat1"]),
                };
                SpatialCoords spt_predictands = new SpatialCoords()
                {
                    nla = int.Parse(form["northernmost_lat2"]),
                    sla = int.Parse(form["southernmost_lat2"]),
                    wlo = int.Parse(form["westernmost_lat2"]),
                    elo = int.Parse(form["easternmost_lat2"]),
                };
                ConfigurationPyCPT confPyCpt = new ConfigurationPyCPT()
                {
                    spatial_predictors = spt_predictors,
                    spatial_predictands = spt_predictands,
                    models = arrmodpycpt,
                    obs = (Obs)int.Parse(form["obs"]),
                    mos = (Mos)int.Parse(form["mos"]),
                    station = bool.Parse(form["station"]),
                    predictand = (Predictand)int.Parse(form["predictand"]),
                    predictors = (Predictors)int.Parse(form["predictors"]),
                    mons = arrmons,
                    tgtii = arrtgtii.ToList(),
                    tgtff = arrtgtff.ToList(),
                    tgts = arrtgts,
                    tini = int.Parse(form["tini"]),
                    tend = int.Parse(form["tend"]),
                    xmodes_min = int.Parse(form["xmodes_min"]),
                    xmodes_max = int.Parse(form["xmodes_max"]),
                    ymodes_min = int.Parse(form["ymodes_min"]),
                    ymodes_max = int.Parse(form["ymodes_max"]),
                    ccamodes_min = int.Parse(form["ccamodes_min"]),
                    ccamodes_max = int.Parse(form["ccamodes_max"]),
                    force_download = bool.Parse(form["force_download"]),
                    single_models = bool.Parse(form["single_models"]),
                    forecast_anomaly = bool.Parse(form["forecast_anomaly"]),
                    forecast_spi = bool.Parse(form["forecast_spi"]),
                    confidence_level = int.Parse(form["confidence_level"]),
                    ind_exec = 1
                };
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
        public async Task<IActionResult> ConfigurationPyCptDelete(string id, string obs, string mos, bool station, string predictand, string predictors, int tini, int tend, int xmodes_min, int xmodes_max, int ymodes_min, int ymodes_max, int ccamodes_min, int ccamodes_max, bool force_download, bool single_models, bool forecast_anomaly, bool forecast_spi, int confidence_level, int ind_exec, DateTime register)
        {
            try
            {
                // Get original crop data
                State entity_new = await db.state.byIdAsync(id);
                // Delete the setup
                await db.state.deleteConfigurationPyCPTAsync(entity_new, (Obs)Enum.Parse(typeof(Obs), obs), (Mos)Enum.Parse(typeof(Mos), mos), station, (Predictand)Enum.Parse(typeof(Predictand), predictand), (Predictors)Enum.Parse(typeof(Predictors), predictors), tini, tend, xmodes_min, xmodes_max, ymodes_min, ymodes_max, ccamodes_min, ccamodes_max, force_download, single_models, forecast_anomaly, forecast_spi, confidence_level, ind_exec, register);
                return RedirectToAction("ConfigurationPyCpt", new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("ConfigurationPyCpt", new { id = id });
            }
        }
    }
}
