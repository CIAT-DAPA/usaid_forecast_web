﻿using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize(Roles = "ADMIN,CLIMATOLOGIST")]
    public class MunicipalityController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public MunicipalityController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, LogEntity.lc_municipality, hostingEnvironment)
        {
        }

        // GET: /Municipality/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.municipality.listEnableAsync();
                writeEvent(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }

        }

        // GET: /Municipality/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Municipality entity = await db.municipality.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // GET: /Municipality/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await generateListStatesAsync(string.Empty);
            return View();
        }

        // POST: /Municipality/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Municipality entity)
        {            
            try
            {
                entity.state = getId(HttpContext.Request.Form["state"].ToString());
                if (ModelState.IsValid)
                {
                    await db.municipality.insertAsync(entity);
                    writeEvent(entity.ToString(), LogEvent.cre);
                    return RedirectToAction("Index");
                }
                writeEvent(ModelState.ToString(), LogEvent.err);
                await generateListStatesAsync(string.Empty);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                await generateListStatesAsync(string.Empty);
                return View(entity);
            }
        }

        // GET: /Municipality/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            Municipality entity=null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.municipality.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }                
                writeEvent("Search id: " + id, LogEvent.rea);
                await generateListStatesAsync(entity.state.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                await generateListStatesAsync(entity.state.ToString());
                return View(entity);
            }
        }

        // POST: /Municipality/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Municipality entity, string id)
        {
            try
            {
                entity.state = getId(HttpContext.Request.Form["state"].ToString());
                if (ModelState.IsValid)
                {
                    Municipality current_entity = await db.municipality.byIdAsync(id);

                    entity.id = getId(id);
                    await db.municipality.updateAsync(current_entity, entity);
                    writeEvent(entity.ToString(), LogEvent.upd);
                    return RedirectToAction("Index");
                }
                writeEvent(ModelState.ToString(), LogEvent.err);
                await generateListStatesAsync(entity.state.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                await generateListStatesAsync(entity.state.ToString());
                return View(entity);
            }
        }

        // GET: /Municipality/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Municipality entity = await db.municipality.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // POST: /Municipality/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Municipality entity = await db.municipality.byIdAsync(id);
                await db.municipality.deleteAsync(entity);
                writeEvent(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }

        /// <summary>
        /// Method that create a select list with the states available
        /// </summary>
        /// <param name="selected">The id of the entity, if it is empty or null, it will takes the first</param>
        private async Task<bool> generateListStatesAsync(string selected)
        {
            var states = (await db.state.listEnableAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            if(string.IsNullOrEmpty(selected))
                ViewData["state"] = new SelectList(states, "id", "name");
            else
                ViewData["state"] = new SelectList(states, "id", "name", selected);
            return states.Count() > 0;
        }
    }
}
