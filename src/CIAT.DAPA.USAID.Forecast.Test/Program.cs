using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
            Console.WriteLine();
            Console.WriteLine("Press Enter");
            Console.ReadLine();
        }        

        static async Task MainAsync(string[] args)
        {
            try
            {
                ForecastDB db = new ForecastDB("mongodb://localhost:27017", "forecast_db");
                // Get parameters of the forecast
                var weatherStations = await db.weatherStation.listEnableVisibleAsync();
                var time = new List<Time>(){ new Time(){ year = 2017, month = 1 }, new Time() { year = 2017, month = 2 },
                    new Time() { year = 2017, month = 3 }, new Time(){ year = 2017, month = 4 }, new Time(){ year = 2017, month = 5 },
                    new Time(){ year = 2017, month = 6 }};
                int[] months = new int[] { 1, 2, 3, 4, 5, 6 };
                // Create a forecast
                var forecast = await db.forecast.insertAsync(new Data.Models.Forecast()
                {
                    start = DateTime.Now,
                    end = DateTime.Now,
                    confidence = 0.5
                });
                // Create forecast climate
                foreach (var ws in weatherStations)
                {
                    var data = new List<ProbabilityClimate>();
                    var performance = new List<PerformanceMetric>();
                    // Probabilities of the forecast
                    foreach (var t in time)
                    {
                        var pc = new ProbabilityClimate() { year = t.year, month = t.month };
                        pc.probabilities = new List<Probability>() { new Probability() { measure = MeasureClimatic.prec, lower = 0.7, normal = 0.2, upper = 0.1 } };
                        data.Add(pc);
                    }
                    performance.Add(new PerformanceMetric() { name = MeasurePerformance.goodnessindex, value = 0.5 });
                    ForecastClimate fc = new ForecastClimate() { forecast = forecast.id, weather_station = ws.id, data = data, performance = performance };
                    await db.forecastClimate.insertAsync(fc);
                }
                // Create forecast yield
                List<HistoricalYieldViewImport> raw = new List<HistoricalYieldViewImport>();
                using (StreamReader sr = File.OpenText(@"D:\SourceCode\USAID\CIAT.DAPA.USAID.Forecast\src\CIAT.DAPA.USAID.Forecast.Test\Data\Forecast.csv"))
                {
                    string line;
                    int lines = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines += 1;
                        if(lines > 1)
                        {
                            string[] values = line.Split(',');
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
                ForecastYield fy_new;
                YieldCrop yc_entity;
                List<YieldCrop> yc_entities;
                List<YieldData> yd_entities;
                foreach (var ws in raw.Select(p => p.weather_station).Distinct())
                {
                    fy_new = new ForecastYield() { forecast = forecast.id, weather_station = ObjectId.Parse(ws) };
                    var yield_crop = raw.Where(p => p.weather_station == ws);
                    yc_entities = new List<YieldCrop>();
                    int count_yc = yield_crop.Count();
                    foreach (var yc in yield_crop)
                    {
                        yc_entity = new YieldCrop() { cultivar = ObjectId.Parse(yc.cultivar), soil = ObjectId.Parse(yc.soil), start = yc.start, end = yc.end };
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
                    fy_new.yield = yc_entities;
                    await db.forecastYield.insertAsync(fy_new);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        class Time
        {
            public int year { get; set; }
            public int month { get; set; }
        }

        public class HistoricalYieldViewImport
        {
            /// <summary>
            /// Id of the weather station
            /// </summary>
            public string weather_station { get; set; }
            /// <summary>
            /// Id of the soil
            /// </summary>
            public string soil { get; set; }
            /// <summary>
            /// Id of the cultivar
            /// </summary>
            public string cultivar { get; set; }
            /// <summary>
            /// Start date
            /// </summary>
            public DateTime start { get; set; }
            /// <summary>
            /// End date
            /// </summary>
            public DateTime end { get; set; }
            /// <summary>
            /// Measure yield
            /// </summary>
            public MeasureYield measure { get; set; }
            /// <summary>
            /// Median
            /// </summary>
            public double median { get; set; }
            /// <summary>
            /// Average
            /// </summary>
            public double avg { get; set; }
            /// <summary>
            /// Minimun value
            /// </summary>
            public double min { get; set; }
            /// <summary>
            /// Maximun value
            /// </summary>
            public double max { get; set; }
            /// <summary>
            /// First quartile
            /// </summary>
            public double quar_1 { get; set; }
            /// <summary>
            /// Second quartile
            /// </summary>
            public double quar_2 { get; set; }
            /// <summary>
            /// Third quartile
            /// </summary>
            public double quar_3 { get; set; }
            /// <summary>
            /// Limit lower confidence
            /// </summary>
            public double conf_lower { get; set; }
            /// <summary>
            /// Limit upper confidence
            /// </summary>
            public double conf_upper { get; set; }
            /// <summary>
            /// Standard desviation
            /// </summary>
            public double sd { get; set; }
            /// <summary>
            /// 5 percentile
            /// </summary>
            public double perc_5 { get; set; }
            /// <summary>
            /// 95 percentile
            /// </summary>
            public double perc_95 { get; set; }
            /// <summary>
            /// coefficient of variation
            /// </summary>
            public double coef_var { get; set; }
        }
    }
}
