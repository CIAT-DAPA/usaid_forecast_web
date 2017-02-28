using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Models;
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
            var f_probabilities = Directory.EnumerateFiles(path + Program.settings.In_PATH_FS_PROBABILITIES);
            List<ProbabilityClimate> probabilities = new List<ProbabilityClimate>();
            foreach (string f in f_probabilities)
            {
                if (f.EndsWith(".csv"))
                {
                    using (file = File.OpenText(f))
                    {
                        int count = 0;
                        while ((line = file.ReadLine()) != null)
                        {
                            if (count != 0)
                            {
                                var fields = line.Split(Program.settings.splitted);
                                probabilities.Add(new ProbabilityClimate()
                                {
                                    year = int.Parse(fields[0]),
                                    month = int.Parse(fields[1]),
                                    
                                });
                            }
                            count += 1;
                        }
                    }
                }
            }

            return true;
        }
    }
}
