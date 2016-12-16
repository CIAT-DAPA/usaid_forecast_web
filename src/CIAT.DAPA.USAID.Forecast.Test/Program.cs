using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
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

        class Time
        {
            public int year { get; set; }
            public int month { get; set; }
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
                string[] lines = File.ReadAllLines(@"D:\SourceCode\USAID\CIAT.DAPA.USAID.Forecast\src\CIAT.DAPA.USAID.Forecast.Test\Data\Forecast.csv");
                //for(int i=0;)
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
