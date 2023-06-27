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
    [Authorize(Roles = "ADMIN, CLIMATOLOGIST")]
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
        // GET: /Country/
        [HttpGet]
        
        public async Task<IActionResult> Index()
        {
            try
            {
                UserPermission permission = await getPermissionAsync();
                var list = await db.country.listEnableAsync();
                // FIlter countries by permission
                list = list.Where(p => permission.countries.Contains(p.id)).ToList();
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
            generateItems(string.Empty, string.Empty);
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
                generateItems(string.Empty, string.Empty);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                generateItems(string.Empty, string.Empty);
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
                generateItems(entity.seasonal_mode.ToString(), entity.subseasonal_mode.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return Redirect("Index");
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
                generateItems(entity.seasonal_mode.ToString(), entity.subseasonal_mode.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                generateItems(entity.seasonal_mode.ToString(), entity.subseasonal_mode.ToString());
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
        [HttpGet]
        
        public async Task<IActionResult> Configuration()
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

        private async Task<IActionResult> LoadPyCptAsync(string id)
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
                generateListsPyCPT();
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // GET: /Country/SeasonalPyCpt/5
        [HttpGet]
        
        public async Task<IActionResult> SeasonalPyCpt(string id)
        {
            return await LoadPyCptAsync(id);
        }

        // GET: /Country/SubseasonalPyCpt/5
        [HttpGet]
        
        public async Task<IActionResult> SubseasonalPyCpt(string id)
        {
            return await LoadPyCptAsync(id);
        }

        /// <summary>
        /// Method that Add new configuration for PyCPT, it can be for seasonal or subseasonal
        /// </summary>
        /// <param name="id">Id country</param>
        /// <param name="form">Form data</param>
        /// <param name="action">action name for redirecting</param>
        /// <param name="type">Type to add</param>
        /// <returns></returns>
        private async Task<IActionResult> PyCptAddAsync(string id, IFormCollection form, string action, TypePyCPT type)
        {
            try
            {
                Country entity = await db.country.byIdAsync(id);
                ConfigurationPyCPT confPyCpt = generatePyCPTConfAsync(form);
                await db.country.addConfigurationPyCpt(entity, confPyCpt, type);
                return RedirectToAction(action, new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction(action, new { id = id });
            }
        }

        // POST: /Country/SeasonalPyCpt/5
        [HttpPost, ActionName("SeasonalPyCpt")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> SeasonalPyCptAdd(string id) 
        {
            return await PyCptAddAsync(id, HttpContext.Request.Form, "SeasonalPyCPT", TypePyCPT.seasonal);
        }

        // POST: /Country/SeasonalPyCpt/5
        [HttpPost, ActionName("SubseasonalPyCpt")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> SubseasonalPyCptAdd(string id)
        {
            return await PyCptAddAsync(id, HttpContext.Request.Form, "SubseasonalPyCPT", TypePyCPT.subseasonal);
        }


        public async Task<IActionResult> PyCptDeleteAsync(string id, int month, long register, string action, TypePyCPT type)
        {
            try
            {
                // Get original country
                Country entity_new = await db.country.byIdAsync(id);
                // Delete the setup
                await db.country.deleteConfigurationPyCPTAsync(entity_new, month, register, type);
                return RedirectToAction(action, new { id = id });
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction(action, new { id = id });
            }
        }

        // POST: /Country/SeasonalPyCptDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> SeasonalPyCptDelete(string id, int month, long register)
        {
            return await PyCptDeleteAsync(id, month, register, "SeasonalPyCPT", TypePyCPT.seasonal);
        }

        // POST: /Country/SubseasonalPyCptDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> SubseasonalPyCptDelete(string id, int month, long register)
        {
            return await PyCptDeleteAsync(id, month, register, "SubseasonalPyCPT", TypePyCPT.subseasonal);
        }

        private void generateItems(string seasonal,string subseasonal)
        {
            var forecast_mode = from ForecastMode d in Enum.GetValues(typeof(ForecastMode))
                             select new { ID = (int)d, Name = d.ToString() };
            ViewData["forecast_seasonal"] = string.IsNullOrEmpty(seasonal) ? 
                                        new SelectList(forecast_mode, "ID", "Name") :
                                        new SelectList(forecast_mode, "ID", "Name", seasonal);
            ViewData["forecast_subseasonal"] = string.IsNullOrEmpty(subseasonal) ?
                                        new SelectList(forecast_mode, "ID", "Name") :
                                        new SelectList(forecast_mode, "ID", "Name", subseasonal);
        }
    }
}
