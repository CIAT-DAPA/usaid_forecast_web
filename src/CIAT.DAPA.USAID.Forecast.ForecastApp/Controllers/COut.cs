using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Controllers
{
    /// <summary>
    /// This Class export data from database
    /// </summary>
    public class COut
    {  
        /// <summary>
        /// Database object
        /// </summary>
        private ForecastDB db { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        public COut()
        {
            db = new ForecastDB(Program.settings.ConnectionString, Program.settings.Database);
        }

        /// <summary>
        /// Method to export the historical data of the weather stations by states
        /// </summary>
        /// <param name="path">Path where the files will located</param>
        /// <param name="measure">Measure to export</param>
        /// <param name="start">Year to start</param>
        /// <param name="end">Year to end</param>
        public async Task<bool> exportStatesHistoricalClimateAsync(string path, MeasureClimatic measure, int start, int end)
        {
            StringBuilder csv;
            string header, line;
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_STATES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES);
            var states = await db.state.listEnableAsync();
            foreach (var s in states)
            {
                Console.WriteLine("Creating " + s.name);
                csv = new StringBuilder();
                var weather_stations = await db.weatherStation.listEnableByStateAsync(s.id);

                // Create the header of file
                header = "year,month,";
                foreach (var ws in weather_stations)
                    header += ws.id.ToString() + ",";
                header = header.Substring(0, header.Length - 1);

                // get historical climate data
                var hc = await db.historicalClimatic.byWeatherStationsAsync(weather_stations.Select(p => p.id).Distinct().ToArray());

                // This code search by every year and month the data of every weather station
                for (int y = start; y <= end; y++)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        line = y.ToString() + "," + i.ToString() + ",";
                        foreach (var ws in weather_stations)
                        {
                            var data_year = hc.SingleOrDefault(p => p.year == y && p.weather_station == ws.id);
                            if (data_year != null)
                            {
                                var data_month = data_year.monthly_data.SingleOrDefault(p => p.month == i);
                                if (data_month != null)
                                {
                                    var data_measure = data_month.data.SingleOrDefault(p => p.measure == measure);
                                    if (data_measure != null)
                                        line += data_measure.value.ToString() + ",";
                                    else
                                        line += "0,";
                                }
                                else
                                    line += "0,";
                            }
                            else
                                line += "0,";
                        }
                        // Add line to file
                        csv.AppendLine(line.Substring(0, line.Length - 1));
                    }
                }
                // Create the physical file
                File.WriteAllText(path + Program.settings.Out_PATH_STATES + @"\" + s.name + ".csv", header + "\n" + csv.ToString());
            }
            return true;
        }

        /// <summary>
        /// Method to export the configuration files by weather station
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        /// <param name="name">Name of file to filter</param>
        public async Task<bool> exportFilesWeatherStationAsync(string path, string name)
        {
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_WS_FILES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_WS_FILES);
            var weather_stations = await db.weatherStation.listEnableAsync();
            foreach (var ws in weather_stations.Where(p => p.visible))
            {
                Console.WriteLine("Exporting " + ws.name);
                var f = ws.conf_files.Where(p => p.name.Equals(name)).OrderByDescending(p => p.date).FirstOrDefault();
                if (f != null)
                    File.Copy(f.path, path + Program.settings.Out_PATH_WS_FILES + @"\" + ws.id.ToString() + "-" + f.name + COut.getExtension(f.path));
                else
                    Console.WriteLine("File not found");
            }
            return true;
        }

        /// <summary>
        /// Method that exports the configuration for the forecast 
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        public async Task<bool> exportForecastSetupnAsync(string path)
        {
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_FS_FILES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_FS_FILES);
            var crops = await db.crop.listEnableAsync();
            foreach (var cp in crops)
            {
                Console.WriteLine("Exporting " + cp.name);
                string dir_crop = path + Program.settings.Out_PATH_FS_FILES + @"\" + cp.name;
                Directory.CreateDirectory(dir_crop);
                foreach (var st in cp.setup.Where(p => p.track.enable))
                {
                    string dir_setup = dir_crop + @"\" + st.weather_station.ToString() + "_" + st.cultivar.ToString() + "_" + st.soil.ToString();
                    Directory.CreateDirectory(dir_setup);
                    foreach (var f in st.conf_files)
                        File.Copy(f.path, dir_setup + @"\" + f.name + COut.getExtension(f.path));
                }
            }
            return true;
        }

        /// <summary>
        /// Method that return the extension name of the file
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <returns></returns>
        public static string getExtension(string path)
        {
            return path.Substring(path.Length - 4, 4);
        }

    }
}