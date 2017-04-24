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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize(Roles = "admin,climatologist")]
    public class StateController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public StateController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, LogEntity.lc_state, hostingEnvironment)
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
                    await db.state.updateAsync(current_entity, entity);
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

        // GET: /State/Import/5
        [HttpGet]
        public async Task<IActionResult> Import(string id)
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
                            latitude = double.Parse(ws_lat.ElementAt(i)),
                            longitude = double.Parse(ws_lon.ElementAt(i)),
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
                    writeEvent(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_state, LogEntity.lc_municipality, LogEntity.lc_weather_station });
                }
                else
                {
                    msg = new Message() { content = "Import MWS. An error occurred with the file imported", type = MessageType.error };
                    writeEvent(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_state, LogEntity.lc_municipality, LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {
                writeException(ex);
                msg = new Message() { content = "Import MWS. An error occurred in the system, contact the administrator", type = MessageType.error };
            }
            ViewBag.message = msg;
            return View("Import", entity);
        }

        
    }
}
