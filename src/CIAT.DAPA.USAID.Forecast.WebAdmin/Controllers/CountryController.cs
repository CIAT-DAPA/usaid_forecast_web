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

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize]
    public class CountryController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public CountryController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) :
            base(settings, LogEntity.lc_country, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }
        // GET: /State/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.country.listEnableAsync();
                await writeEventAsync(list.Count().ToString(), LogEvent.lis);

                return View(list);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }
        // GET: /Country/Details/1
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
                Country entity = await db.country.byIdAsync(id);
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
        public ActionResult Create()
        {
            return View();
        }
        // POST: /Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await db.country.insertAsync(entity);
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
        // GET: /State/Edit/5
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
                Country entity = await db.country.byIdAsync(id);
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
        // POST: /Country/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country entity, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Country current_entity = await db.country.byIdAsync(id);

                    entity.id = getId(id);
                    await db.country.updateAsync(current_entity, entity);
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
                Country entity = await db.country.byIdAsync(id);
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
        // POST: /Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Country entity = await db.country.byIdAsync(id);
                await db.country.deleteAsync(entity);
                await writeEventAsync(entity.ToString(), LogEvent.del);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Delete", new { id = id });
            }
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
                Country entity = await db.country.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                ViewBag.country = entity;
                IEnumerable<dynamic> buleans = new List<dynamic> { new { id = 0, name = "True"}, new { id = 1, name = "False"} };
                ViewBag.ind_exec = new SelectList(buleans, "id", "name");
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
                           select new { id = (int)q, name = q.ToString()};
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
            List<SelectListItem> checkst = new List<SelectListItem>();
            // List climate variables
            var mons = from Mons q in Enum.GetValues(typeof(Mons))
                             select new { id = (int)q, name = q.ToString() };
            var qua = from Quarter q in Enum.GetValues(typeof(Quarter))
                      select new { id = (int)q, name = q.ToString() };
            foreach (var item in mons)
            {
                checks2.Add(new SelectListItem { Text = item.name.ToString(), Value = item.id.ToString() });
            }
            foreach (var item in qua)
            {
                checkst.Add(new SelectListItem { Text = item.name.ToString(), Value = item.id.ToString() });
            }
            ViewBag.mons = checks2;
            ViewBag.tgts = checkst;
        }
        // POST: /Country/ConfigurationPyCpt/5
        [HttpPost, ActionName("ConfigurationPyCpt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurationPyCptAdd(string id) 
        {
            try
            {
                var form = HttpContext.Request.Form;
                Country entity = await db.country.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                List<Quarter> arrtgts = new List<Quarter>();
                foreach (var item in form["tgts"])
                {
                    if (item != "false")
                    {
                        arrtgts.Add((Quarter)int.Parse(item));
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
                    arrtgtii.Add(form["tgtii_value_"+f.ToString()]);
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
                    ind_exec = int.Parse(form["ind_exec"])
                };
                await db.country.addConfigurationPyCpt(entity, confPyCpt);

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
                Country entity_new = await db.country.byIdAsync(id);
                // Delete the setup
                await db.country.deleteConfigurationPyCPTAsync(entity_new, (Obs)Enum.Parse(typeof(Obs), obs), (Mos)Enum.Parse(typeof(Mos), mos), station, (Predictand)Enum.Parse(typeof(Predictand), predictand), (Predictors)Enum.Parse(typeof(Predictors), predictors), tini, tend, xmodes_min, xmodes_max, ymodes_min, ymodes_max, ccamodes_min, ccamodes_max, force_download, single_models, forecast_anomaly, forecast_spi, confidence_level, ind_exec, register);
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
