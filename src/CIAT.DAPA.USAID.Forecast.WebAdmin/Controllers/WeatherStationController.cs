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
using MongoDB.Bson;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Extend;
using CIAT.DAPA.USAID.Forecast.Data.Database;

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
            generateListMunicipalitiesAsync(string.Empty);
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
                generateListMunicipalitiesAsync(string.Empty);
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                generateListMunicipalitiesAsync(string.Empty);
                return View(entity);
            }
        }

        // GET: /WeatherStation/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            WeatherStation entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                writeEvent("Search id: " + id, LogEvent.rea);
                generateListMunicipalitiesAsync(entity.municipality.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                generateListMunicipalitiesAsync(entity.municipality.ToString());
                return View(entity);
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
                generateListMunicipalitiesAsync(entity.municipality.ToString());
                return View(entity);
            }
            catch (Exception ex)
            {
                writeException(ex);
                generateListMunicipalitiesAsync(entity.municipality.ToString());
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
                    await file.CopyToAsync(new FileStream(importPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-weatherstation-historical-climate-ws-" + file.FileName, FileMode.Create));
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
                            if (ws_entity != null)
                            {
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
                    }
                    msg = new Message()
                    {
                        content = "Historical Climate WS. The file was imported correctly. Records imported: (" + (lines - 1).ToString() + ")  rows",
                        type = MessageType.successful
                    };
                    writeEvent(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_weather_station, LogEntity.hs_historical_climatic });
                }
                else
                {
                    msg = new Message() { content = "Historical Climate WS. An error occurred with the file imported", type = MessageType.error };
                    writeEvent(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {
                writeException(ex);
                msg = new Message() { content = "Historical Climate WS. An error occurred in the system, contact the administrator", type = MessageType.error };
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
                    await file.CopyToAsync(new FileStream(importPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-weatherstation-climatology-" + file.FileName, FileMode.Create));
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
                        if (ws_entity != null)
                        {
                            cl_entity = await db.climatology.byWeatherStationAsync(ws_entity.id);
                            if (cl_entity == null)
                                cl_new = new Climatology() { weather_station = ws_entity.id, monthly_data = new List<MonthlyDataStation>() };
                            else
                                cl_new = new Climatology() { id = cl_entity.id, weather_station = cl_entity.weather_station, monthly_data = cl_entity.monthly_data };
                            var months = ws_values.Select(p => p.month).Distinct();
                            foreach (var m in months)
                            {
                                var monthlyData = cl_new.monthly_data.FirstOrDefault(p => p.month == m) ?? new MonthlyDataStation() { month = m, data = new List<ClimaticData>() };
                                var restMonthlyData = cl_new.monthly_data.Where(p => p.month != m).ToList() ?? new List<MonthlyDataStation>();
                                data = monthlyData.data.ToList();
                                data.AddRange(ws_values.Where(p => p.month == m).Select(p => new ClimaticData() { measure = p.measure, value = p.value }));
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

        // POST: /State/ImportHistoricalYield
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportHistoricalYield(string source, IFormFile file)
        {
            Message msg = null;
            try
            {
                if (file != null && file.Length > 0)
                {
                    // Save a copy in the web site
                    await file.CopyToAsync(new FileStream(importPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-weatherstation-historical-yield-ws-" + file.FileName, FileMode.Create));
                    // Read the file
                    StreamReader reader = new StreamReader(file.OpenReadStream());
                    string line = string.Empty;
                    int lines = 0;
                    List<HistoricalYieldViewImport> raw = new List<HistoricalYieldViewImport>();
                    string[] values = null;
                    // Read the file
                    while (!reader.EndOfStream)
                    {
                        line = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            lines += 1;
                            // Don't reade the headers
                            if (lines != 1)
                            {
                                // Fixed the values to view import historical climate
                                // The first two columns are the year and month
                                values = line.Split(',');
                                raw.Add(new HistoricalYieldViewImport()
                                {
                                    weather_station = values[0],
                                    soil = values[1],
                                    cultivar = values[2],
                                    start = DateTime.Parse(values[3]),
                                    end = DateTime.Parse(values[4]),
                                    measure = (MeasureYield)Enum.Parse(typeof(MeasureYield), values[5]),
                                    median = double.Parse(values[6]),
                                    avg = double.Parse(values[7]),
                                    min = double.Parse(values[8]),
                                    max = double.Parse(values[9]),
                                    quar_1 = double.Parse(values[10]),
                                    quar_2 = double.Parse(values[11]),
                                    quar_3 = double.Parse(values[12]),
                                    conf_lower = double.Parse(values[13]),
                                    conf_upper = double.Parse(values[14]),
                                    sd = double.Parse(values[15]),
                                    perc_5 = double.Parse(values[16]),
                                    perc_95 = double.Parse(values[17]),
                                    coef_var = double.Parse(values[18])
                                });
                            }
                        }
                    }
                    // Import to the database
                    HistoricalYield hy_new, hy_entity;
                    YieldCrop yc_entity;
                    List<YieldCrop> yc_entities;
                    List<YieldData> yd_entities;
                    // En esta sección se crean los nuevos registros de rendimientos para las estaciones climatologicas.
                    // Se obtiene la información de las estaciones climátologicas, luego se filtra
                    // 
                    // 
                    foreach (var ws in raw.Select(p => p.weather_station).Distinct())
                    {
                        /*
                        hy_entity = await db.historicalYield.byWeatherStationSourceAsync(ForecastDB.parseId(ws), source);
                        if (hy_entity == null)
                            hy_new = new HistoricalYield() { source = source, weather_station = getId(ws) };
                        else
                            hy_new = new HistoricalYield() { id = hy_entity.id, source = hy_entity.source, weather_station = hy_entity.weather_station, yield = hy_entity.yield };*/
                        hy_new = new HistoricalYield() { source = source, weather_station = getId(ws) };
                        var yield_crop = raw.Where(p => p.weather_station == ws);
                        yc_entities = new List<YieldCrop>();
                        //int count_yc = yield_crop.Count();
                        foreach (var yc in yield_crop)
                        {
                            yc_entity = new YieldCrop() { cultivar = getId(yc.cultivar), soil = getId(yc.soil), start = yc.start, end = yc.end };
                            var yield_data = yield_crop.Where(p => p.cultivar == yc.cultivar && p.soil == yc.soil && p.start == yc.start && p.end == yc.end);
                            int count_yd = yield_data.Count();
                            yd_entities = new List<YieldData>();
                            foreach (var yd in yield_data)
                                yd_entities.Add(new YieldData()
                                {
                                    measure = yd.measure,
                                    median = yd.median,
                                    avg = yd.avg,
                                    min = yd.min,
                                    max = yd.max,
                                    quar_1 = yd.quar_1,
                                    quar_2 = yd.quar_2,
                                    quar_3 = yd.quar_3,
                                    conf_lower = yd.conf_lower,
                                    conf_upper = yd.conf_upper,
                                    sd = yd.sd,
                                    perc_5 = yd.perc_5,
                                    perc_95 = yd.perc_95,
                                    coef_var = yd.coef_var
                                });
                            yc_entity.data = yd_entities;
                            yc_entities.Add(yc_entity);
                        }
                        hy_new.yield = yc_entities;
                        await db.historicalYield.insertAsync(hy_new);
                        /*
                        if (hy_entity == null)
                            await db.historicalYield.insertAsync(hy_new);
                        else
                            await db.historicalYield.updateAsync(hy_entity, hy_new);*/

                    }
                    msg = new Message()
                    {
                        content = "Historical Yield WS. The file was imported correctly. Records imported: (" + (lines - 1).ToString() + ")  rows",
                        type = MessageType.successful
                    };
                    writeEvent(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_weather_station, LogEntity.hs_historical_yield });
                }
                else
                {
                    msg = new Message() { content = "Historical Yield WS. An error occurred with the file imported", type = MessageType.error };
                    writeEvent(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {
                writeException(ex);
                msg = new Message() { content = "Historical Yield WS. An error occurred in the system, contact the administrator", type = MessageType.error };
            }
            // List climate variables
            generateListMeasures();
            ViewBag.message = msg;
            return View("Import");
        }

        // GET: /WeatherStation/Range/5
        [HttpGet]
        public async Task<IActionResult> Range(string id)
        {
            WeatherStation entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                // Set data for the view
                ViewBag.ws_name = entity.name;
                ViewBag.ws_id = entity.id;
                // Get data 
                var cp = await db.crop.listEnableAsync();
                // 
                List<CropYieldRange> entities = new List<CropYieldRange>();
                foreach (var r in entity.ranges)
                    entities.Add(new CropYieldRange(r, cp));
                // Fill the select list
                ViewBag.crop = new SelectList(cp, "id", "name");
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entities);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // POST: /WeatherStation/Range/5
        [HttpPost, ActionName("Range")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RangeAdd(string id)
        {
            try
            {
                // Get original weather station data
                var form = HttpContext.Request.Form;
                WeatherStation entity_new = await db.weatherStation.byIdAsync(id);
                // Instance the new range entity
                YieldRange range = new YieldRange()
                {
                    crop = getId(form["crop"]),
                    label = form["label"],
                    lower = int.Parse(form["lower"]),
                    upper = int.Parse(form["upper"])
                };
                await db.weatherStation.addRangeAsync(entity_new, range);
                writeEvent(id + "range add: " + range.crop.ToString() + "-" + range.label + "-" + range.lower.ToString() + "-" + range.upper.ToString(), LogEvent.upd);
                return RedirectToAction("Range", new { id = id });
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Range", new { id = id });
            }
        }

        // POST: /WeatherStation/RangeDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RangeDelete(string ws, string crop, string label, int lower, int upper)
        {
            try
            {
                // Get original crop data
                WeatherStation entity_new = await db.weatherStation.byIdAsync(ws);
                // Delete the setup
                await db.weatherStation.deleteRangeAsync(entity_new, crop, label, lower, upper);
                writeEvent(ws + "range del: " + crop + "-" + label + "-" + lower.ToString() + "-" + upper.ToString(), LogEvent.upd);
                return RedirectToAction("Range", new { id = ws });
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Range", new { id = ws });
            }
        }

        // GET: /WeatherStation/Range/5
        [HttpGet]
        public async Task<IActionResult> Configuration(string id)
        {
            WeatherStation entity = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    writeEvent("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                entity = await db.weatherStation.byIdAsync(id);
                if (entity == null)
                {
                    writeEvent("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                // Set data for the view
                ViewBag.ws_name = entity.name;
                ViewBag.ws_id = entity.id;
                // 
                var entities = entity.conf_files;
                writeEvent("Search id: " + id, LogEvent.rea);
                return View(entities);
            }
            catch (Exception ex)
            {
                writeException(ex);
                return View();
            }
        }

        // POST: /WeatherStation/Configuration/5
        [HttpPost, ActionName("Configuration")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurationAdd(string id)
        {
            try
            {
                // Get original weather station data
                var form = HttpContext.Request.Form;
                WeatherStation entity_new = await db.weatherStation.byIdAsync(id);
                // Instance the new range entity
                ConfigurationFile file_temp = new ConfigurationFile()
                {
                    date = DateTime.Now,
                    path = configurationPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "-wsconf-" + id + "-" + form.Files[0].FileName,
                    name = form["name"]
                };
                // Save a copy of the file in the server
                using (var stream = new FileStream(file_temp.path, FileMode.Create))
                {
                    stream.Position = 0;
                    await form.Files[0].CopyToAsync(stream);
                }               

                await db.weatherStation.addConfigurationFileAsync(entity_new, file_temp);
                writeEvent(id + "file add: " + entity_new.id.ToString() + "-" + form.Files[0].FileName, LogEvent.upd);
                return RedirectToAction("Configuration", new { id = id });
            }
            catch (Exception ex)
            {
                writeException(ex);
                return RedirectToAction("Configuration", new { id = id });
            }
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

        /// <summary>
        /// Method that create a select list with the municipalities available
        /// </summary>
        /// <param name="selected">The id of the entity, if it is empty or null, it will takes the first</param>
        private async void generateListMunicipalitiesAsync(string selected)
        {
            var municipalities = (await db.municipality.listEnableAsync()).Select(p => new { id = p.id.ToString(), name = p.name });
            if (string.IsNullOrEmpty(selected))
                ViewData["municipality"] = new SelectList(municipalities, "id", "name");
            else
                ViewData["municipality"] = new SelectList(municipalities, "id", "name", selected);
        }
    }
}
