using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import;
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

            // Create a forecast
            var forecast = await db.forecast.insertAsync(new Data.Models.Forecast()
            {
                start = DateTime.Now,
                end = DateTime.Now,
                confidence = cf
            });

            // Load probabilities
            // Get all files from directory
            var f_probabilities = Directory.EnumerateFiles(path + Program.settings.In_PATH_FS_PROBABILITIES);
            List<ImportProbability> probabilities = new List<ImportProbability>();
            // This cicle goes through all files in the directory
            foreach (string f in f_probabilities)
            {
                // Filter only the csv files
                if (f.EndsWith(".csv"))
                {
                    // Reading the file
                    using (file = File.OpenText(f))
                    {
                        int count = 0;
                        while ((line = file.ReadLine()) != null)
                        {
                            if (count != 0)
                            {
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
            }
            // Create the records of the probabilities in the database
            foreach (var ws in probabilities.Select(p => p.ws).Distinct())
                await db.forecastClimate.insertAsync(new ForecastClimate()
                {
                    forecast = forecast.id,
                    weather_station = ForecastDB.parseId(ws),
                    data = probabilities.Where(p => p.Equals(ws)).Select(p => new ProbabilityClimate()
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
                    }).ToList()
                });

            // Load scenarios
            // Get folder of the scenarios



            return true;
        }
    }
}
