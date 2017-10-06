using CIAT.DAPA.USAID.Forecast.Data.Database;
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
            Console.WriteLine(path + Program.settings.In_PATH_FS_RASTER_SOURCE);
            if (Directory.Exists(Program.settings.In_PATH_FS_RASTER_DESTINATION))
            {
                string raster_d = Program.settings.In_PATH_FS_RASTER_DESTINATION + @"\" + forecast.id.ToString();
                DirectoryHelper.DirectoryCopy(path + Program.settings.In_PATH_FS_RASTER_SOURCE, raster_d, true);
            }

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
                                    start = DateTime.Parse(fields[3]),
                                    end = DateTime.Parse(fields[4]),
                                    measure = fields[5],
                                    median = double.Parse(fields[6]),
                                    avg = double.Parse(fields[7]),
                                    min = double.Parse(fields[8]),
                                    max = double.Parse(fields[9]),
                                    quar_1 = double.Parse(fields[10]),
                                    quar_2 = double.Parse(fields[11]),
                                    quar_3 = double.Parse(fields[12]),
                                    conf_lower = double.Parse(fields[13]),
                                    conf_upper = double.Parse(fields[14]),
                                    sd = double.Parse(fields[15]),
                                    perc_5 = double.Parse(fields[16]),
                                    perc_95 = double.Parse(fields[17]),
                                    coef_var = double.Parse(fields[18])
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
            foreach (var ws in yields.Select(p => p.weather_station).Distinct())
            {
                fy_new = new ForecastYield() { forecast = forecast.id, weather_station = ForecastDB.parseId(ws) };
                var yield_crop = yields.Where(p => p.weather_station == ws);
                yc_entities = new List<YieldCrop>();
                int count_yc = yield_crop.Count();
                foreach (var yc in yield_crop)
                {
                    yc_entity = new YieldCrop() { cultivar = ForecastDB.parseId(yc.cultivar), soil = ForecastDB.parseId(yc.soil), start = yc.start, end = yc.end };
                    var yield_data = yield_crop.Where(p => p.cultivar == yc.cultivar && p.soil == yc.soil && p.start == yc.start && p.end == yc.end);
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
    }
}
