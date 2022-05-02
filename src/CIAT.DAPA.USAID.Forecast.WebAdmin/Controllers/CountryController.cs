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
                generateListsPyCPT();
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
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
                ConfigurationPyCPT confPyCpt = generatePyCPTConfAsync(form);
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
        public async Task<IActionResult> ConfigurationPyCptDelete(string id, int month, long register)
        {
            try
            {
                // Get original crop data
                Country entity_new = await db.country.byIdAsync(id);
                // Delete the setup
                await db.country.deleteConfigurationPyCPTAsync(entity_new, month, register);
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
