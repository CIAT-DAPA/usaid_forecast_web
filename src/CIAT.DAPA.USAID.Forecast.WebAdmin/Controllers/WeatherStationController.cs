using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Import;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    public class WeatherStationController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public WeatherStationController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, LogEntity.lc_weather_station, hostingEnvironment)
        {
        }

        // GET: /WeatherStation/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await db.weatherStation.listEnableAsync();
                writeEvent(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }

        }

        // GET: /WeatherStation/Details/5
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
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
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

        // GET: /WeatherStation/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var municipalities = await db.municipality.listEnableAsync();
            ViewBag.municipality = new SelectList(municipalities, "id", "name");
            return View();
        }

        // POST: /WeatherStation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WeatherStation entity)
        {
            try
            {
                entity.municipality = getId(HttpContext.Request.Form["municipality"].ToString());
                if (ModelState.IsValid)
                {
                    await db.weatherStation.insertAsync(entity);
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

        // GET: /WeatherStation/Edit/5
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
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                var municipalities = await db.municipality.listEnableAsync();
                ViewBag.municipality = new SelectList(municipalities, "id", "name");
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // POST: /WeatherStation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WeatherStation entity, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    WeatherStation current_entity = await db.weatherStation.byIdAsync(id);

                    entity.id = getId(id);
                    entity.municipality = getId(HttpContext.Request.Form["municipality"].ToString());
                    await db.weatherStation.updateAsync(current_entity, entity);
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

        // GET: /WeatherStation/Delete/5
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
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
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

        // POST: /WeatherStation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                WeatherStation entity = await db.weatherStation.byIdAsync(id);
                await db.weatherStation.deleteAsync(entity);
                writeEvent(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }

        // GET: /WeatherStation/
        [HttpGet]
        public async Task<IActionResult> Import()
        {
            try
            {
                // List climate variables
                generateListMeasures();
                return View();
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Index");
            }

        }

        // POST: /State/ImportHistoricalClimate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportHistoricalClimate(int search, int measures, IFormFile file)
        {
            Message msg = null;
            try
            {
                if (file != null && file.Length > 0)
                {
                    // Save a copy in the web site
                    await file.CopyToAsync(new FileStream(importPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-weatherstation-historicalws-" + file.FileName, FileMode.Create));
                    // Read the file
                    StreamReader reader = new StreamReader(file.OpenReadStream());
                    string line = string.Empty;
                    int lines = 0;
                    IEnumerable<WeatherStation> ws = null;
                    List<HistoricalClimateViewImport> raw = new List<HistoricalClimateViewImport>();
                    string[] patterns = null;
                    string[] values = null;
                    // Read the file
                    while (!reader.EndOfStream)
                    {
                        line = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            lines += 1;
                            // Reading the headers
                            if (lines == 1)
                            {
                                patterns = line.Split(',').Skip(2).ToArray();
                                // Searching the weather stations according of the parameter to seek
                                if (search == 1)
                                    ws = await db.weatherStation.listEnableByExtIDsAsync(patterns);
                                else if (search == 2)
                                    ws = await db.weatherStation.listEnableByNamesAsync(patterns);
                            }
                            else
                            {
                                // Fixed the values to view import historical climate
                                // The first two columns are the year and month
                                values = line.Split(',');
                                for (int i = 2; i < values.Length; i++)
                                    raw.Add(new HistoricalClimateViewImport()
                                    {
                                        year = int.Parse(values[0]),
                                        month = int.Parse(values[1]),
                                        ext_id = search == 1 ? patterns[i - 2] : string.Empty,
                                        name = search == 2 ? patterns[i - 2] : string.Empty,
                                        value = double.Parse(values[i])
                                    });
                            }
                        }
                    }
                    // Import to the database
                    WeatherStation ws_entity;
                    HistoricalClimatic hc_entity, hc_new;
                    MeasureClimatic mc = (MeasureClimatic)measures;
                    List<ClimaticData> data;
                    // In this section it is filtered the data of the field loaded and the historical information is added to the data base.
                    // We first get the ids of the climate stations, then it is filtered the information of each year and each station. 
                    // With this information we look for in the data base if it historical information stored, in case that it is not created in 
                    // a new identity. It is filtered the information of every month in order to add the data to the field.
                    // At the end it is updated the information. If there is not a file, it is created as a new one.
                    var ws_patterns = search == 1 ? raw.Select(p => p.ext_id).Distinct() : raw.Select(p => p.name).Distinct();
                    foreach (var y in raw.Select(p => p.year).Distinct())
                    {
                        foreach (var ws_p in ws_patterns)
                        {
                            var ws_values = search == 1 ? raw.Where(p => p.ext_id == ws_p && p.year == y) : raw.Where(p => p.name == ws_p && p.year == y);
                            ws_entity = search == 1 ? ws.FirstOrDefault(p => p.ext_id == ws_p) : ws.FirstOrDefault(p => p.name == ws_p);
                            hc_entity = await db.historicalClimatic.byYearWeatherStationAsync(y, ws_entity.id);
                            if (hc_entity == null)
                                hc_new = new HistoricalClimatic() { weather_station = ws_entity.id, year = y, monthly_data = new List<MonthlyDataStation>() };
                            else
                                hc_new = new HistoricalClimatic() { id = hc_entity.id, weather_station = hc_entity.weather_station, year = y, monthly_data = hc_entity.monthly_data };
                            var months = ws_values.Select(p => p.month);
                            foreach (var m in months)
                            {
                                var monthlyData = hc_new.monthly_data.FirstOrDefault(p => p.month == m) ?? new MonthlyDataStation() { month = m, data = new List<ClimaticData>() };
                                var restMonthlyData = hc_new.monthly_data.Where(p => p.month != m).ToList() ?? new List<MonthlyDataStation>();
                                data = monthlyData.data.ToList();
                                data.Add(ws_values.Where(p => p.month == m).Select(p => new ClimaticData() { measure = mc, value = p.value }).FirstOrDefault());
                                monthlyData.data = data;
                                restMonthlyData.Add(monthlyData);
                                hc_new.monthly_data = restMonthlyData;
                            }
                            // In case that the entity didn't exist, it will be created in the database
                            if (hc_entity == null)
                                await db.historicalClimatic.insertAsync(hc_new);
                            else
                                await db.historicalClimatic.updateAsync(hc_entity, hc_new);

                        }
                    }
                    msg = new Message()
                    {
                        content = "Historical WS. The file was imported correctly. Records imported: (" + (lines - 1).ToString() + ")  rows",
                        type = MessageType.successful
                    };
                    writeEvent(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_weather_station, LogEntity.hs_historical_climatic });
                }
                else
                {
                    msg = new Message() { content = "Historical WS. An error occurred with the file imported", type = MessageType.error };
                    writeEvent(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {
                writeException(ex);
                msg = new Message() { content = "Historical WS. An error occurred in the system, contact the administrator", type = MessageType.error };
            }
            // List climate variables
            generateListMeasures();
            ViewBag.message = msg;
            return View("Import");
        }

        // POST: /State/ImportClimatology
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportClimatology(int search, IFormFile file)
        {
            Message msg = null;
            try
            {
                if (file != null && file.Length > 0)
                {
                    // Save a copy in the web site
                    await file.CopyToAsync(new FileStream(importPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-weatherstation-historicalws-" + file.FileName, FileMode.Create));
                    // Read the file
                    StreamReader reader = new StreamReader(file.OpenReadStream());
                    string line = string.Empty;
                    int lines = 0;
                    IEnumerable<WeatherStation> ws = null;
                    List<ClimatologyViewImport> raw = new List<ClimatologyViewImport>();
                    string[] patterns = null;
                    string[] values = null;
                    // Read the file
                    while (!reader.EndOfStream)
                    {
                        line = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            lines += 1;
                            // Reading the headers
                            if (lines == 1)
                            {
                                patterns = line.Split(',').Skip(2).ToArray();
                                // Searching the weather stations according of the parameter to seek
                                if (search == 1)
                                    ws = await db.weatherStation.listEnableByExtIDsAsync(patterns);
                                else if (search == 2)
                                    ws = await db.weatherStation.listEnableByNamesAsync(patterns);
                            }
                            else
                            {
                                // Fixed the values to view import historical climate
                                // The first two columns are the year and month
                                values = line.Split(',');
                                for (int i = 2; i < values.Length; i++)
                                    raw.Add(new ClimatologyViewImport()
                                    {
                                        measure = (MeasureClimatic)Enum.Parse(typeof(MeasureClimatic), values[0]),
                                        month = int.Parse(values[1]),
                                        ext_id = search == 1 ? patterns[i - 2] : string.Empty,
                                        name = search == 2 ? patterns[i - 2] : string.Empty,
                                        value = double.Parse(values[i])
                                    });
                            }
                        }
                    }
                    // Import to the database
                    WeatherStation ws_entity;
                    Climatology cl_entity, cl_new;
                    List<ClimaticData> data;
                    // In this section it is filtered the data of the field loaded and the historical information is added to the data base.
                    // We first get the ids of the climate stations, then it is filtered the information of each station. 
                    // With this information we look for in the data base if it historical information stored, in case that it is not created in 
                    // a new identity. It is filtered the information of every month in order to add the data to the field.
                    // At the end it is updated the information. If there is not a file, it is created as a new one.
                    var ws_patterns = search == 1 ? raw.Select(p => p.ext_id).Distinct() : raw.Select(p => p.name).Distinct();
                    foreach (var ws_p in ws_patterns)
                    {
                        var ws_values = search == 1 ? raw.Where(p => p.ext_id == ws_p) : raw.Where(p => p.name == ws_p);
                        ws_entity = search == 1 ? ws.FirstOrDefault(p => p.ext_id == ws_p) : ws.FirstOrDefault(p => p.name == ws_p);
                        cl_entity = await db.climatology.byWeatherStationAsync(ws_entity.id);
                        if (cl_entity == null)
                            cl_new = new Climatology() {  weather_station = ws_entity.id, monthly_data = new List<MonthlyDataStation>() };
                        else
                            cl_new = new Climatology() { id = cl_entity.id, weather_station = cl_entity.weather_station, monthly_data = cl_entity.monthly_data };
                        var months = ws_values.Select(p => p.month);
                        foreach (var m in months)
                        {
                            var monthlyData = cl_new.monthly_data.FirstOrDefault(p => p.month == m) ?? new MonthlyDataStation() { month = m, data = new List<ClimaticData>() };
                            var restMonthlyData = cl_new.monthly_data.Where(p => p.month != m).ToList() ?? new List<MonthlyDataStation>();
                            data = monthlyData.data.ToList();
                            data.Add(ws_values.Where(p => p.month == m).Select(p => new ClimaticData() { measure = p.measure, value = p.value }).FirstOrDefault());
                            monthlyData.data = data;
                            restMonthlyData.Add(monthlyData);
                            cl_new.monthly_data = restMonthlyData;
                        }
                        // In case that the entity didn't exist, it will be created in the database
                        if (cl_entity == null)
                            await db.climatology.insertAsync(cl_new);
                        else
                            await db.climatology.updateAsync(cl_entity, cl_new);

                    }
                    msg = new Message()
                    {
                        content = "Climatology WS. The file was imported correctly. Records imported: (" + (lines - 1).ToString() + ")  rows",
                        type = MessageType.successful
                    };
                    writeEvent(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_weather_station, LogEntity.hs_climatology });
                }
                else
                {
                    msg = new Message() { content = "Climatology WS. An error occurred with the file imported", type = MessageType.error };
                    writeEvent(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {
                writeException(ex);
                msg = new Message() { content = "Climatology WS. An error occurred in the system, contact the administrator", type = MessageType.error };
            }
            // List climate variables
            generateListMeasures();
            ViewBag.message = msg;
            return View("Import");
        }

        /// <summary>
        /// Method to create a select list with the measure available to import
        /// </summary>
        private void generateListMeasures()
        {
            // List climate variables
            var measures = from MeasureClimatic mc in Enum.GetValues(typeof(MeasureClimatic))
                           select new { id = (int)mc, name = mc.ToString() };
            ViewBag.measures = new SelectList(measures, "id", "name");
        }
    }
}
