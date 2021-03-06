﻿using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import;
using CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Controllers
{
    public class CIn
    {
        /// <summary>
        /// Database object
        /// </summary>
        private ForecastDB db { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        public CIn()
        {
            db = new ForecastDB(Program.settings.ConnectionString, Program.settings.Database);
        }

        public async Task<bool> importForecastAsync(string path, double cf)
        {
            StreamReader file;
            string line;
            // Load the avaliable cpt configuration by state
            List<ClimateConfiguration> climate_conf = new List<ClimateConfiguration>();
            var states = await db.state.listEnableAsync();
            foreach (var s in states)
                climate_conf.Add(new ClimateConfiguration() { state = s.id, conf = s.conf.ToList().Where(p => p.track.enable) });
            // Create a forecast            
            var forecast = await db.forecast.insertAsync(new Data.Models.Forecast()
            {
                start = DateTime.Now,
                end = DateTime.Now,
                confidence = cf,
                climate_conf = climate_conf
            });
            Console.WriteLine("Created forecast " + forecast.id.ToString());
            // Load probabilities
            Console.WriteLine("Getting probabilities and performance");
            Console.WriteLine(path + Program.settings.In_PATH_FS_CLIMATE + @"\" + Program.settings.In_PATH_FS_PROBABILITIES);
            var f_probabilities = Directory.EnumerateFiles(path + Program.settings.In_PATH_FS_CLIMATE + @"\" + Program.settings.In_PATH_FS_PROBABILITIES);
            // Get the probabilities file
            List<ImportProbability> probabilities = new List<ImportProbability>();
            string fp = f_probabilities.SingleOrDefault(p => p.Contains(Program.settings.In_PATH_FS_FILE_PROBABILITY));
            if (!string.IsNullOrEmpty(fp))
            {
                Console.WriteLine("Processing: probabilities");
                Console.WriteLine(fp);
                // Reading the file
                using (file = File.OpenText(fp))
                {
                    int count = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        // Omitted the file's header
                        if (count != 0 && !string.IsNullOrEmpty(line))
                        {
                            // Get the probabilities from the file in a temp memmory
                            var fields = line.Split(Program.settings.splitted);
                            probabilities.Add(new ImportProbability()
                            {
                                year = int.Parse(fields[0]),
                                month = int.Parse(fields[1]),
                                ws = fields[2],
                                below = double.Parse(fields[3]),
                                normal = double.Parse(fields[4]),
                                above = double.Parse(fields[5])
                            });
                        }
                        count += 1;
                    }
                }
            }
            else
                Console.WriteLine("Probabilities not found");
            // Get the performance metrics file
            List<ImportPerformance> performances = new List<ImportPerformance>();
            string fpe = f_probabilities.SingleOrDefault(p => p.Contains(Program.settings.In_PATH_FS_FILE_PERFORMANCE));
            if (!string.IsNullOrEmpty(fpe))
            {
                Console.WriteLine("Processing: performance");
                Console.WriteLine(fpe);
                // Reading the file
                using (file = File.OpenText(fpe))
                {
                    int count = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        // Omitted the file's header
                        if (count != 0 && !string.IsNullOrEmpty(line))
                        {
                            // Get the probabilities from the file in a temp memmory
                            var fields = line.Split(Program.settings.splitted);
                            performances.Add(new ImportPerformance()
                            {
                                year = int.Parse(fields[0]),
                                month = int.Parse(fields[1]),
                                ws = fields[2],
                                pearson = double.Parse(fields[3]),
                                kendall = double.Parse(fields[4]),
                                goodness = double.Parse(fields[5])
                            });
                        }
                        count += 1;
                    }
                }
            }
            else
                Console.WriteLine("Performances not found");
            // Create the records of the probabilities in the database
            Console.WriteLine("Saving the probabilities and metrics in the database");
            foreach (var ws in probabilities.Select(p => p.ws).Distinct())
            {
                // Casting the metrics
                List<PerformanceMetric> metrics = new List<PerformanceMetric>();
                foreach (var m in Enum.GetValues(typeof(MeasurePerformance)).Cast<MeasurePerformance>())
                {
                    if (m == MeasurePerformance.goodness)
                        metrics.AddRange(performances.Where(p => p.ws.Equals(ws)).Select(p => new PerformanceMetric()
                        {
                            year = p.year,
                            month = p.month,
                            name = m,
                            value = p.goodness
                        }).ToList());
                    else if (m == MeasurePerformance.kendall)
                        metrics.AddRange(performances.Where(p => p.ws.Equals(ws)).Select(p => new PerformanceMetric()
                        {
                            year = p.year,
                            month = p.month,
                            name = m,
                            value = p.kendall
                        }).ToList());
                    else if (m == MeasurePerformance.pearson)
                        metrics.AddRange(performances.Where(p => p.ws.Equals(ws)).Select(p => new PerformanceMetric()
                        {
                            year = p.year,
                            month = p.month,
                            name = m,
                            value = p.pearson
                        }).ToList());
                }
                // Saving in the database
                await db.forecastClimate.insertAsync(new ForecastClimate()
                {
                    forecast = forecast.id,
                    weather_station = ForecastDB.parseId(ws),
                    data = probabilities.Where(p => p.ws.Equals(ws)).Select(p => new ProbabilityClimate()
                    {
                        year = p.year,
                        month = p.month,
                        probabilities = new List<Probability>()
                        {
                            new Probability()
                            {
                                measure = MeasureClimatic.prec,
                                lower = p.below,
                                normal = p.normal,
                                upper = p.above
                            }
                        }
                    }).OrderBy(p => p.year).ThenBy(p => p.month).ToList(),
                    performance = metrics.OrderBy(p => p.year).ThenBy(p => p.month).ToList()
                });
            }


            // Load scenarios
            Console.WriteLine("Getting scenarios");
            // Get folder of the scenarios
            List<ImportScenario> scenarios = new List<ImportScenario>();
            Console.WriteLine("Searching scenarios");
            Console.WriteLine(path + Program.settings.In_PATH_FS_CLIMATE + @"\" + Program.settings.In_PATH_FS_SCENARIOS);
            // Get a list of files of the scenarios 
            var d_f_scenario = Directory.EnumerateFiles(path + Program.settings.In_PATH_FS_CLIMATE + @"\" + Program.settings.In_PATH_FS_SCENARIOS).Where(p => !p.Contains("scenario")).OrderBy(p => p);
            // This cicle goes through of the scenarios (max, min, avg)
            foreach (var s in Enum.GetNames(typeof(ScenarioName)))
            {
                // This cicle goes through of the measure (t_max, t_min, )
                foreach (var me in Enum.GetNames(typeof(MeasureClimatic)))
                {
                    var f_scenarios = d_f_scenario.Where(p => p.Contains(me + "_" + s));
                    if (f_scenarios.Count() == 0)
                        Console.WriteLine("File not found. scenario: " + s + " measure: " + me);
                    foreach (var fs in f_scenarios)
                    {
                        Console.WriteLine("Getting scenario: " + s + " measure: " + me);
                        Console.WriteLine(fs);
                        var ws_fs = fs.Split('\\')[fs.Split('\\').Length - 1].Substring(0, 24);
                        Console.WriteLine(ws_fs);
                        // Reading the file
                        using (file = File.OpenText(fs))
                        {
                            int count = 0;
                            while ((line = file.ReadLine()) != null)
                            {
                                // Omitted the file's header
                                if (count != 0 && !string.IsNullOrEmpty(line))
                                {
                                    // Get the data from the file in a temp memmory
                                    var fields = line.Split(Program.settings.splitted);
                                    scenarios.Add(new ImportScenario()
                                    {
                                        year = int.Parse(fields[0]),
                                        month = int.Parse(fields[1]),
                                        ws = ws_fs,
                                        scenario = s,
                                        measure = me,
                                        value = double.Parse(fields[2])
                                    });
                                }
                                count += 1;
                            }
                        }

                    }
                }
            }
            // Create the records of the scenarios in the database
            Console.WriteLine("Saving the scenarios in the database");
            var header_scenario = scenarios.Select(p => new
            {
                p.ws,
                p.year,
                p.scenario,
            }).Distinct().OrderBy(p => p.ws).ThenBy(p => p.year);
            foreach (var data in header_scenario)
            {
                var data_temp = scenarios.Where(p => p.ws == data.ws && p.scenario == data.scenario && p.year == data.year).
                                            OrderBy(p => p.measure).ThenBy(p => p.month);
                List<MonthlyDataStation> monthly_data = new List<MonthlyDataStation>();
                foreach (var month in data_temp.Select(p => p.month).Distinct())
                {
                    monthly_data.Add(new MonthlyDataStation()
                    {
                        month = month,
                        data = data_temp.Where(p => p.month == month).Select(p2 => new ClimaticData()
                        {
                            measure = (MeasureClimatic)Enum.Parse(typeof(MeasureClimatic), p2.measure, true),
                            value = p2.value
                        }).ToList()
                    });
                }
                await db.forecastScenario.insertAsync(new ForecastScenario()
                {
                    forecast = forecast.id,
                    weather_station = ForecastDB.parseId(data.ws),
                    name = (ScenarioName)Enum.Parse(typeof(ScenarioName), data.scenario, true),
                    year = data.year,
                    monthly_data = monthly_data
                });
            }

            // Load yield data
            Console.WriteLine("Copying raster");
            Console.WriteLine(path + Program.settings.In_PATH_FS_CLIMATE + @"\" + Program.settings.In_PATH_FS_RASTER_SOURCE);
            if (Directory.Exists(Program.settings.In_PATH_FS_RASTER_DESTINATION))
            {
                string raster_d = Program.settings.In_PATH_FS_RASTER_DESTINATION + @"\" + forecast.id.ToString();
                DirectoryHelper.DirectoryCopy(path + Program.settings.In_PATH_FS_CLIMATE + @"\" + Program.settings.In_PATH_FS_RASTER_SOURCE, raster_d, true);
            }
            else
                Console.WriteLine("Folder to save the raster files doesn't exist: " + Program.settings.In_PATH_FS_RASTER_DESTINATION);

            // Load yield data
            Console.WriteLine("Getting yield");
            Console.WriteLine(path + Program.settings.In_PATH_FS_YIELD);
            // Get folder of the scenarios
            var d_crops = Directory.EnumerateDirectories(path + Program.settings.In_PATH_FS_YIELD);
            List<ImportYield> yields = new List<ImportYield>();
            foreach (var dc in d_crops)
            {
                Console.WriteLine("Crop " + dc);
                var d_f_yield = Directory.EnumerateFiles(dc);
                foreach (var f_yield in d_f_yield.Where(p => p.EndsWith(".csv")))
                {
                    Console.WriteLine("Working in " + f_yield);
                    using (file = File.OpenText(f_yield))
                    {
                        int count = 0;
                        while ((line = file.ReadLine()) != null)
                        {
                            // Omitted the file's header
                            if (count != 0 && !string.IsNullOrEmpty(line))
                            {
                                // Get the data from the file in a temp memmory
                                var fields = line.Split(Program.settings.splitted);
                                yields.Add(new ImportYield()
                                {
                                    weather_station = fields[0],
                                    soil = fields[1],
                                    cultivar = fields[2],
                                    start = Program.settings.Add_Day ? DateTime.Parse(fields[3]).AddDays(1) : DateTime.Parse(fields[3]),
                                    end = Program.settings.Add_Day ? DateTime.Parse(fields[4]).AddDays(1) : DateTime.Parse(fields[4]),
                                    measure = fields[5],
                                    avg = double.Parse(fields[6].ToLower().Equals("nan") ? "0" : fields[6]),
                                    median = double.Parse(fields[7].ToLower().Equals("nan") ? "0" : fields[7]),                                    
                                    min = double.Parse(fields[8].ToLower().Equals("nan") ? "0" : fields[8]),
                                    max = double.Parse(fields[9].ToLower().Equals("nan") ? "0" : fields[9]),
                                    quar_1 = double.Parse(fields[10].ToLower().Equals("nan") ? "0" : fields[10]),
                                    quar_2 = double.Parse(fields[11].ToLower().Equals("nan") ? "0" : fields[11]),
                                    quar_3 = double.Parse(fields[12].ToLower().Equals("nan") ? "0" : fields[12]),
                                    conf_lower = double.Parse(fields[13].ToLower().Equals("nan") ? "0" : fields[13]),
                                    conf_upper = double.Parse(fields[14].ToLower().Equals("nan") ? "0" : fields[14]),
                                    sd = double.Parse(fields[15].ToLower().Equals("nan") ? "0" : fields[15]),
                                    perc_5 = double.Parse(fields[16].ToLower().Equals("nan") ? "0" : fields[16]),
                                    perc_95 = double.Parse(fields[17].ToLower().Equals("nan") ? "0" : fields[17]),
                                    coef_var = double.Parse(fields[18].ToLower().Equals("nan") ? "0" : fields[18])
                                });
                            }
                            count += 1;
                        }
                    }
                }
            }
            // Create the records of the yield in the database
            Console.WriteLine("Saving the yield in the database");
            ForecastYield fy_new;
            YieldCrop yc_entity;
            List<YieldCrop> yc_entities;
            List<YieldData> yd_entities;
            var ws_list = yields.Select(p => new { p.weather_station, p.cultivar, p.soil }).Distinct();
            foreach (var ws in ws_list)
            {
                Console.WriteLine("Working in ws: " + ws.weather_station + " soil: " + ws.soil + " cultivar: " + ws.cultivar);
                fy_new = new ForecastYield()
                {
                    forecast = forecast.id,
                    weather_station = ForecastDB.parseId(ws.weather_station),
                    cultivar = ForecastDB.parseId(ws.cultivar),
                    soil = ForecastDB.parseId(ws.soil)

                };
                var yield_crop = yields.Where(p => p.weather_station == ws.weather_station && p.soil == ws.soil && p.cultivar == ws.cultivar);
                yc_entities = new List<YieldCrop>();
                var dates_list = yield_crop.Select(p => new { start = p.start, end = p.end }).Distinct();
                int count_yc = yield_crop.Count();
                foreach (var dc in dates_list)
                {
                    yc_entity = new YieldCrop() { start = dc.start, end = dc.end };
                    var yield_data = yield_crop.Where(p => p.start == dc.start && p.end == dc.end);
                    int count_yd = yield_data.Count();
                    yd_entities = new List<YieldData>();
                    foreach (var yd in yield_data)
                        yd_entities.Add(new YieldData()
                        {
                            measure = (MeasureYield)Enum.Parse(typeof(MeasureYield), yd.measure, true),
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
                fy_new.yield = yc_entities;
                await db.forecastYield.insertAsync(fy_new);
            }

            Console.WriteLine("Forecast imported");
            return true;
        }

        public async Task<bool> importHistoricalAsync(string path, MeasureClimatic mc, int search)
        {
            StreamReader file;
            string line = string.Empty;
            int lines = 0;
            IEnumerable<WeatherStation> ws = null;
            List<HistoricalClimateViewImport> raw = new List<HistoricalClimateViewImport>();
            string[] patterns = null;
            string[] values = null;

            Console.WriteLine("Importing: " + path);
            // Read the file
            using (file = File.OpenText(path))
            {
                // Read line
                Console.WriteLine("Loading data");
                while ((line = file.ReadLine()) != null)
                {
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
                Console.WriteLine("Data loaded: " + raw.Count.ToString());
                // Import to the database
                WeatherStation ws_entity;
                HistoricalClimatic hc_entity, hc_new;
                List<ClimaticData> data;
                // In this section it is filtered the data of the field loaded and the historical information is added to the data base.
                // We first get the ids of the climate stations, then it is filtered the information of each year and each station. 
                // With this information we look for in the data base if it historical information stored, in case that it is not created in 
                // a new identity. It is filtered the information of every month in order to add the data to the field.
                // At the end it is updated the information. If there is not a file, it is created as a new one.
                var ws_patterns = search == 1 ? raw.Select(p => p.ext_id).Distinct() : raw.Select(p => p.name).Distinct();
                foreach (int y in raw.Select(p => p.year).Distinct())
                {
                    foreach (string ws_p in ws_patterns)
                    {
                        Console.WriteLine("Processing: " + ws_p + " year: " + y.ToString());
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

            }
            Console.WriteLine("Import process has finished");
            return true;
        }
    }
}
