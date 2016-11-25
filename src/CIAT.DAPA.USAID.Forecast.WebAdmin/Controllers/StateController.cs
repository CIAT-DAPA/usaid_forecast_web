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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    public class StateController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        public StateController(IOptions<Settings> settings) : base(settings, LogEntity.lc_state)
        {
        }

        // GET: /State/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.state.listEnableAsync();
                writeEvent(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }

        }

        // GET: /State/Details/5
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
                State entity = await db.state.byIdAsync(id);
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

        // GET: /State/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /State/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(State entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await db.state.insertAsync(entity);
                    writeEvent(entity.ToString(), LogEvent.cre);
                    return RedirectToAction("Index");
                }
                writeEvent(ModelState.ToString(), LogEvent.err);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
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
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                State entity = await db.state.byIdAsync(id);
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

        // POST: /State/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(State entity, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    State current_entity = await db.state.byIdAsync(id);

                    entity.id = getId(id);
                    await db.state.updateAsync(current_entity,entity);
                    writeEvent(entity.ToString(), LogEvent.upd);
                    return RedirectToAction("Index");
                }
                writeEvent(ModelState.ToString(), LogEvent.err);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
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
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                State entity = await db.state.byIdAsync(id);
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

        // POST: /State/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                State entity = await db.state.byIdAsync(id);
                await db.state.deleteAsync(entity);
                writeEvent(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }
    }
}
