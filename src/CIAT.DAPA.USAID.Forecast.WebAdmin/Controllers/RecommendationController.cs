﻿using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    //[Authorize(Roles = "ADMIN,IMPROVER")]
    [Authorize]
    public class RecommendationController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public RecommendationController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) :
            base(settings, LogEntity.cp_recommendation, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }

        private async Task<PermissionList> LoadEnableByPermissionAsync()
        {
            UserPermission permission = await getPermissionAsync();
            PermissionList val = new PermissionList();
            val.countries = await db.country.listEnableAsync();
            val.countries = val.countries.Where(p => permission.countries.Contains(p.id)).ToList();
            val.recommendations = await db.recommendation.listAsync();
            val.recommendations = val.recommendations.Where(p => permission.countries.Contains(p.country)).ToList();
            return val;
        }

        // GET: /Recommendation/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                PermissionList obj = await LoadEnableByPermissionAsync();
                List<Recommendation> list = obj.recommendations;
                ViewBag.countries = obj.countries;

                List<string> enums = new List<string>();
                foreach (MeasureYield en in (MeasureYield[])Enum.GetValues(typeof(MeasureYield)))
                {
                    if (en.ToString().Contains("st_") || en.ToString().Contains("hs_") || en.ToString() == "land_pre_day")
                        enums.Add(en.ToString());
                }
                enums.Add("best_planting_date");
                enums.Add("best_cultivar");

                List<string> resp = new List<string>();
                foreach (RecommendationType en in (RecommendationType[])Enum.GetValues(typeof(RecommendationType)))
                {
                    resp.Add(en.ToString());
                }

                List<string> langs = new List<string>();
                foreach (RecommendationLang lang in (RecommendationLang[])Enum.GetValues(typeof(RecommendationLang)))
                {
                    langs.Add(lang.ToString());
                }

                ViewBag.enums = enums;

                ViewBag.langs = langs;

                ViewBag.type_resp = resp;

                await writeEventAsync(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }

        }

        // GET: /Recommendation/Details/5
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
                Recommendation entity = await db.recommendation.byIdAsync(id);
                ViewBag.country = await db.country.byIdAsync(entity.country.ToString());
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

        // GET: /Recommendation/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await generateListCountriesAsync("");
            await generateListEnumAsync("");
            await generateListRespAsync("");
            await generateListLangAsync("");
            return View();
        }

        // POST: /Recommendation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recommendation entity)
        {
            try
            {
                entity.country = getId(HttpContext.Request.Form["country"].ToString());
                entity.type_enum = HttpContext.Request.Form["type_enum"].ToString();
                entity.type_resp = HttpContext.Request.Form["type_resp"].ToString();
                entity.lang = HttpContext.Request.Form["lang"].ToString();
                if (ModelState.IsValid)
                {
                    await db.recommendation.insertAsync(entity);
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

        // GET: /Recommendation/Edit/5
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
                Recommendation entity = await db.recommendation.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await generateListCountriesAsync(entity.country.ToString());
                await generateListEnumAsync(entity.type_enum.ToString());
                await generateListRespAsync(entity.type_resp.ToString());
                await generateListLangAsync(entity.lang.ToString());
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /Recommendation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Recommendation entity, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Recommendation current_entity = await db.recommendation.byIdAsync(id);

                    entity.id = getId(id);
                    entity.country = getId(HttpContext.Request.Form["country"].ToString());
                    await db.recommendation.updateAsync(current_entity, entity);
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

        // GET: /Recommendation/Delete/5
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
                Recommendation entity = await db.recommendation.byIdAsync(id);
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

        // POST: /Recommendation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Recommendation entity = await db.recommendation.byIdAsync(id);
                await db.recommendation.deleteAsync(entity);
                await writeEventAsync(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Delete", new { id = id });
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

        private async Task<bool> generateListEnumAsync(string selected)
        {
            // Filter states by permission by countries            
            List<string> enums = new List<string>();
            foreach (MeasureYield en in (MeasureYield[])Enum.GetValues(typeof(MeasureYield)))
            {
                if (en.ToString().Contains("st_") || en.ToString().Contains("hs_") || en.ToString() == "land_pre_day")
                    enums.Add(en.ToString());
            }
            enums.Add("best_planting_date");
            enums.Add("best_cultivar");
            if (string.IsNullOrEmpty(selected))
                ViewData["enum"] = new SelectList(enums);
            else
                ViewData["enum"] = new SelectList(enums, selected);
            return enums.Count() > 0;
        }

        private async Task<bool> generateListRespAsync(string selected)
        {
            // Filter states by permission by countries            
            List<string> resp = new List<string>();
            foreach (RecommendationType en in (RecommendationType[])Enum.GetValues(typeof(RecommendationType)))
            {
                resp.Add(en.ToString());
            }
            if (string.IsNullOrEmpty(selected))
                ViewData["resp"] = new SelectList(resp);
            else
                ViewData["resp"] = new SelectList(resp, selected);
            return resp.Count() > 0;
        }

        private async Task<bool> generateListLangAsync(string selected)
        {
            // Filter states by permission by countries            
            List<string> resp = new List<string>();
            foreach (RecommendationLang lang in (RecommendationLang[])Enum.GetValues(typeof(RecommendationLang)))
            {
                resp.Add(lang.ToString());
            }
            if (string.IsNullOrEmpty(selected))
                ViewData["lang"] = new SelectList(resp);
            else
                ViewData["lang"] = new SelectList(resp, selected);
            return resp.Count() > 0;
        }
    }
}
