using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Import;
using System.IO;
using System.Globalization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize(Roles = "ADMIN,CLIMATOLOGIST,IMPROVER")]
    [Authorize]
    public class ImportController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public ImportController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) :
            base(settings, LogEntity.lc_weather_station, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }

        // GET: /Import/ImportHistoricalClimate
        [HttpGet]
        public async Task<IActionResult> HistoricalClimate()
        {
            try
            {
                // List climate variables
                await generateListMeasuresAndSourceAsync();

                return View();
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Import/ImportHistoricalClimate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HistoricalClimate(int search, int measures, IFormFile file)
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
                                patterns = line.Replace("\"", "").Split(',').Skip(2).ToArray();
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
                                        value = double.Parse(values[i], CultureInfo.InvariantCulture)
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
                    await writeEventAsync(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_weather_station, LogEntity.hs_historical_climatic });
                }
                else
                {
                    msg = new Message() { content = "Historical Climate WS. An error occurred with the file imported", type = MessageType.error };
                    await writeEventAsync(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                msg = new Message() { content = "Historical Climate WS. An error occurred in the system, contact the administrator", type = MessageType.error };
            }
            // List climate variables
            await generateListMeasuresAndSourceAsync();
            ViewBag.message = msg;
            return View("HistoricalClimate");
        }

        // GET: /Import/ImportClimatology
        [HttpGet]
        public async Task<IActionResult> Climatology()
        {
            try
            {
                // List climate variables
                await generateListMeasuresAndSourceAsync();

                return View();
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Import/ImportClimatology
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Climatology(int search, IFormFile file)
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
                                patterns = line.Replace("\"", "").Split(',').Skip(2).ToArray();
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
                                values = line.Replace("\"", "").Split(',');
                                for (int i = 2; i < values.Length; i++)
                                    raw.Add(new ClimatologyViewImport()
                                    {
                                        measure = (MeasureClimatic)Enum.Parse(typeof(MeasureClimatic), values[0]),
                                        month = int.Parse(values[1]),
                                        ext_id = search == 1 ? patterns[i - 2] : string.Empty,
                                        name = search == 2 ? patterns[i - 2] : string.Empty,
                                        value = double.Parse(values[i], CultureInfo.InvariantCulture)
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
                    await writeEventAsync(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_weather_station, LogEntity.hs_climatology });
                }
                else
                {
                    msg = new Message() { content = "Climatology WS. An error occurred with the file imported", type = MessageType.error };
                    await writeEventAsync(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                msg = new Message() { content = "Climatology WS. An error occurred in the system, contact the administrator", type = MessageType.error };
            }
            // List climate variables
            await generateListMeasuresAndSourceAsync();
            ViewBag.message = msg;
            return View("Climatology");
        }

        // GET: /Import/ImportClimatology
        [HttpGet]
        public async Task<IActionResult> HistoricalYield()
        {
            try
            {
                // List climate variables
                await generateListMeasuresAndSourceAsync();

                return View();
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Import/ImportHistoricalYield
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HistoricalYield(string source, IFormFile file)
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
                    int lines = 0, total_lines = 0;
                    List<HistoricalYieldViewImport> raw = new List<HistoricalYieldViewImport>();
                    string[] values = null;
                    // Read the file
                    while (!reader.EndOfStream)
                    {
                        line = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            lines += 1;
                            total_lines += 1;
                            // Don't reade the headers
                            if (lines != 1)
                            {
                                try
                                {
                                    // Fixed the values to view import historical climate
                                    // The first two columns are the year and month
                                    values = line.Split(',');
                                    raw.Add(new HistoricalYieldViewImport()
                                    {
                                        weather_station = values[0],
                                        soil = values[1],
                                        cultivar = values[2],
                                        start = DateTime.SpecifyKind(DateTime.Parse(values[3]), DateTimeKind.Utc),
                                        end = DateTime.SpecifyKind(DateTime.Parse(values[4]), DateTimeKind.Utc),
                                        measure = (MeasureYield)Enum.Parse(typeof(MeasureYield), values[5]),
                                        median = double.Parse(values[6],CultureInfo.InvariantCulture),
                                        avg = double.Parse(values[7], CultureInfo.InvariantCulture),
                                        min = double.Parse(values[8], CultureInfo.InvariantCulture),
                                        max = double.Parse(values[9], CultureInfo.InvariantCulture),
                                        quar_1 = double.Parse(values[10], CultureInfo.InvariantCulture),
                                        quar_2 = double.Parse(values[11], CultureInfo.InvariantCulture),
                                        quar_3 = double.Parse(values[12], CultureInfo.InvariantCulture),
                                        conf_lower = double.Parse(values[13], CultureInfo.InvariantCulture),
                                        conf_upper = double.Parse(values[14], CultureInfo.InvariantCulture),
                                        sd = double.Parse(values[15], CultureInfo.InvariantCulture),
                                        perc_5 = double.Parse(values[16], CultureInfo.InvariantCulture),
                                        perc_95 = double.Parse(values[17], CultureInfo.InvariantCulture),
                                        coef_var = double.Parse(values[18], CultureInfo.InvariantCulture)
                                    });
                                }
                                catch (Exception ex){
                                    lines -= 1;
                                    Console.WriteLine(ex);
                                }                                
                            }
                        }
                    }
                    // Import to the database
                    HistoricalYield hy_new;
                    YieldCrop yc_entity;
                    List<YieldCrop> yc_entities;
                    List<YieldData> yd_entities;
                    // En esta sección se crean los nuevos registros de rendimientos para las estaciones climatologicas.
                    foreach (var ws in raw.Select(p => new { p.weather_station, p.cultivar, p.soil }).Distinct())
                    {
                        hy_new = new HistoricalYield() { source = getId(source), weather_station = getId(ws.weather_station), soil = getId(ws.soil), cultivar = getId(ws.cultivar) };
                        var yield_crop = raw.Where(p => p.weather_station == ws.weather_station && p.soil == ws.soil && p.cultivar == ws.cultivar);
                        yc_entities = new List<YieldCrop>();
                        foreach (var yc in yield_crop)
                        {
                            yc_entity = new YieldCrop() { start = yc.start, end = yc.end };
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
                    }
                    msg = new Message()
                    {
                        content = "Historical Yield WS. The file was imported correctly. Records imported: " + (lines - 1).ToString() + " from: " + (total_lines - 1).ToString() + "  rows",
                        type = MessageType.successful
                    };
                    await writeEventAsync(msg.content, LogEvent.cre, new List<LogEntity>() { LogEntity.lc_weather_station, LogEntity.hs_historical_yield });
                }
                else
                {
                    msg = new Message() { content = "Historical Yield WS. An error occurred with the file imported", type = MessageType.error };
                    await writeEventAsync(msg.content, LogEvent.err, new List<LogEntity>() { LogEntity.lc_weather_station });
                }
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                msg = new Message() { content = "Historical Yield WS. An error occurred in the system, contact the administrator", type = MessageType.error };
            }
            // List climate variables
            await generateListMeasuresAndSourceAsync();
            ViewBag.message = msg;
            return View("HistoricalYield");
        }

        /// <summary>
        /// Method to create a select list with the measure available to import
        /// </summary>
        private async Task generateListMeasuresAndSourceAsync()
        {
            // List climate variables
            var measures = from MeasureClimatic mc in Enum.GetValues(typeof(MeasureClimatic))
                           select new { id = (int)mc, name = mc.ToString() };
            ViewBag.measures = new SelectList(measures, "id", "name");
            // List sources
            var sources = await db.source.listEnableAsync();
            ViewBag.source = new SelectList(sources, "id", "name");
        }
    }
}
