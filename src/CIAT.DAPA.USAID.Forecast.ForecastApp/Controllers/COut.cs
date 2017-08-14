using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.ForecastApp.Util;
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
                if (!Directory.Exists(path + Program.settings.Out_PATH_STATES + @"\" + s.id.ToString()))
                    Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES + @"\" + s.id.ToString());
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
                string file_name = path + Program.settings.Out_PATH_STATES + @"\" + s.id.ToString() + @"\stations" + ".csv";
                if (File.Exists(file_name))
                    File.Delete(file_name);
                File.WriteAllText(file_name, header + "\n" + csv.ToString());
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
                Console.WriteLine("Exporting files ws: " + ws.name);
                var f = ws.conf_files.Where(p => p.name.Equals(name)).OrderByDescending(p => p.date).FirstOrDefault();
                if (f != null)
                    File.Copy(f.path, path + Program.settings.Out_PATH_WS_FILES + @"\" + ws.id.ToString() + COut.getExtension(f.path), true);
                else
                    Console.WriteLine("File not found");
            }
            return true;
        }

        /// <summary>
        /// Method to export the configuration files by weather station
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        public async Task<bool> exportCoordsWeatherStationAsync(string path)
        {
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_WS_FILES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_WS_FILES);
            var weather_stations = await db.weatherStation.listEnableAsync();
            foreach (var ws in weather_stations.Where(p => p.visible))
            {
                Console.WriteLine("Exporting coords ws: " + ws.name);
                StringBuilder coords = new StringBuilder();
                coords.Append("lat,lon\n");
                coords.Append(ws.latitude.ToString() + "," + ws.longitude.ToString() + "\n");
                File.WriteAllText(path + Program.settings.Out_PATH_WS_FILES + @"\" + ws.id.ToString() + "_coords.csv", coords.ToString());
            }
            return true;
        }

        /// <summary>
        /// Method that exports the configuration for the forecast 
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        public async Task<bool> exportForecastSetupAsync(string path)
        {
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_FS_FILES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_FS_FILES);
            var crops = await db.crop.listEnableAsync();
            foreach (var cp in crops)
            {
                Console.WriteLine("Exporting " + cp.name);
                string dir_crop = path + Program.settings.Out_PATH_FS_FILES + @"\" + Tools.folderCropName(cp.name);
                Directory.CreateDirectory(dir_crop);
                foreach (var st in cp.setup.Where(p => p.track.enable))
                {
                    string dir_setup = dir_crop + @"\" + st.weather_station.ToString() + "_" + st.cultivar.ToString() + "_" + st.soil.ToString() + "_" + st.days.ToString();
                    Directory.CreateDirectory(dir_setup);
                    foreach (var f in st.conf_files)
                        File.Copy(f.path, dir_setup + @"\" + f.name + COut.getExtension(f.path), true);
                    // Add csv file with geolocation for rice crop only
                    if (Program.settings.Out_CROPS_COORDINATES.Contains(Tools.folderCropName(cp.name)))
                    {
                        WeatherStation ws = await db.weatherStation.byIdAsync(st.weather_station.ToString());
                        StringBuilder coords = new StringBuilder();
                        coords.Append("name,value\n");
                        coords.Append("lat," + ws.latitude.ToString() + "\n");
                        coords.Append("long," + ws.longitude.ToString() + "\n");
                        coords.Append("elev," + ws.elevation.ToString() + "\n");
                        File.WriteAllText(dir_setup + @"\" + Program.settings.Out_PATH_FILE_COORDINATES, coords.ToString());
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Method that export all cpt  configuration needs by the forecast
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        public async Task<bool> exportCPTSetupAsync(string path)
        {
            StringBuilder header_cpt, x_m, y_m, cca, gamma, header_areas;
            StringBuilder[] x1, x2, y1, y2;
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_STATES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES);
            var states = await db.state.listEnableAsync();
            foreach (var s in states)
            {
                Console.WriteLine("Creating " + s.name);
                if (!Directory.Exists(path + Program.settings.Out_PATH_STATES + @"\" + s.id.ToString()))
                    Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES + @"\" + s.id.ToString());

                // the cpt configuration 
                header_cpt = new StringBuilder("var,");
                x_m = new StringBuilder("x_modes,");
                y_m = new StringBuilder("y_modes,");
                cca = new StringBuilder("cca_modes,");
                gamma = new StringBuilder("trasformation,");
                // the regions configurations
                header_areas = new StringBuilder("order,var,");
                x1 = new StringBuilder[] { new StringBuilder("1,x1,"), new StringBuilder("2,x1,") };
                x2 = new StringBuilder[] { new StringBuilder("1,x2,"), new StringBuilder("2,x2,") };
                y1 = new StringBuilder[] { new StringBuilder("1,y1,"), new StringBuilder("2,y1,") };
                y2 = new StringBuilder[] { new StringBuilder("1,y2,"), new StringBuilder("2,y2,") };
                // This cicle is by every quarter of year
                foreach (string q in Enum.GetNames(typeof(Quarter)))
                {
                    var conf = s.conf.SingleOrDefault(p => p.trimester == (Quarter)Enum.Parse(typeof(Quarter), q));
                    // the cpt configuration 
                    header_cpt.Append(q + ",");
                    x_m.Append((conf.x_mode.ToString() ?? string.Empty) + ",");
                    y_m.Append(conf.y_mode.ToString() + ",");
                    cca.Append(conf.cca_mode.ToString() + ",");
                    gamma.Append(conf.gamma.ToString() + ",");
                    // the regions configurations
                    header_areas.Append(q + ",");
                    x1[0].Append(conf.regions.ElementAt(0).left_lower.lon.ToString() + ",");
                    x2[0].Append(conf.regions.ElementAt(0).rigth_upper.lon.ToString() + ",");
                    y1[0].Append(conf.regions.ElementAt(0).left_lower.lat.ToString() + ",");
                    y2[0].Append(conf.regions.ElementAt(0).rigth_upper.lat.ToString() + ",");
                    // Second region
                    if (conf.regions.Count() > 1)
                    {
                        x1[1].Append(conf.regions.ElementAt(1).left_lower.lon.ToString() + ",");
                        x2[1].Append(conf.regions.ElementAt(1).rigth_upper.lon.ToString() + ",");
                        y1[1].Append(conf.regions.ElementAt(1).left_lower.lat.ToString() + ",");
                        y2[1].Append(conf.regions.ElementAt(1).rigth_upper.lat.ToString() + ",");
                    }
                    else
                    {
                        x1[1].Append("NA,");
                        x2[1].Append("NA,");
                        y1[1].Append("NA,");
                        y2[1].Append("NA,");
                    }
                }
                // Create the physical file cpt
                string file_name_cpt = path + Program.settings.Out_PATH_STATES + @"\" + s.id.ToString() + @"\cpt" + ".csv";
                if (File.Exists(file_name_cpt))
                    File.Delete(file_name_cpt);
                File.WriteAllText(file_name_cpt, header_cpt.ToString().Substring(0, header_cpt.ToString().Length - 1) + "\n" +
                    x_m.ToString().Substring(0, x_m.ToString().Length - 1) + "\n" +
                    y_m.ToString().Substring(0, y_m.ToString().Length - 1) + "\n" +
                    cca.ToString().Substring(0, cca.ToString().Length - 1) + "\n" +
                    gamma.ToString().Substring(0, gamma.ToString().Length - 1));

                // Create the physical file regions
                string file_name_regions = path + Program.settings.Out_PATH_STATES + @"\" + s.id.ToString() + @"\areas" + ".csv";
                if (File.Exists(file_name_regions))
                    File.Delete(file_name_regions);
                File.WriteAllText(file_name_regions, header_areas.ToString().Substring(0, header_areas.ToString().Length - 1) + "\n" +
                    x1[0].ToString().Substring(0, x1[0].ToString().Length - 1) + "\n" +
                    x2[0].ToString().Substring(0, x2[0].ToString().Length - 1) + "\n" +
                    y1[0].ToString().Substring(0, y1[0].ToString().Length - 1) + "\n" +
                    y2[0].ToString().Substring(0, y2[0].ToString().Length - 1) + "\n" +
                    x1[1].ToString().Substring(0, x1[1].ToString().Length - 1) + "\n" +
                    x2[1].ToString().Substring(0, x2[1].ToString().Length - 1) + "\n" +
                    y1[1].ToString().Substring(0, y1[1].ToString().Length - 1) + "\n" +
                    y2[1].ToString().Substring(0, y2[1].ToString().Length - 1) + "\n");

                // Create the theorical areas file
                Console.WriteLine("Creating regions " + s.name);

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