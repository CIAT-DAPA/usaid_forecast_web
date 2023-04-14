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
using Newtonsoft.Json;
using CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Export;

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
        /// Gets the list of names for months
        /// </summary>
        private string[] months { get { return new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }; } }

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
        public async Task<bool> exportStatesHistoricalClimateAsync(string path, MeasureClimatic measure, int start, int end, string mainCountry)
        {
            StringBuilder csv;
            string header, line;
            Console.WriteLine("Exporting in: " + path);
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_STATES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES);
            var states = await db.state.listEnableAsync();
            IEnumerable<State> statesByCountry = states.Where(p => p.country.ToString() == mainCountry);
            foreach (var s in statesByCountry)
            {
                Console.WriteLine("Creating " + s.name);
                if (!Directory.Exists(path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString()))
                    Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString());
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
                                        line += ",";
                                }
                                else
                                    line += ",";
                            }
                            else
                                line += ",";
                        }
                        // Add line to file
                        csv.AppendLine(line.Substring(0, line.Length - 1));
                    }
                }
                // Create the physical file                
                string file_name = path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString() + Path.DirectorySeparatorChar + "stations" + ".csv";
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
        public async Task<bool> exportFilesWeatherStationAsync(string path, string name, string mainCountry)
        {
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_WS_FILES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_WS_FILES);
            var weather_stations = await db.weatherStation.listEnableAsync();
            var dir_def = "data_configuration/";
            foreach (var ws in weather_stations.Where(p => p.visible && p.conf_files.Count() > 0))
            {
                var municipality = await db.municipality.byIdAsync(ws.municipality.ToString());
                var state = await db.state.byIdAsync(municipality.state.ToString());
                var country = await db.country.byIdAsync(state.country.ToString());
                if (country.id.ToString() == mainCountry)
                {
                    Console.WriteLine("Exporting files ws: " + ws.name);
                    var f = ws.conf_files.Where(p => p.name.Equals(name)).OrderByDescending(p => p.date).FirstOrDefault();
                    if (f != null)
                    {
                            File.Copy(f.path, path + Program.settings.Out_PATH_WS_FILES + Path.DirectorySeparatorChar + ws.id.ToString() + COut.getExtension(f.path), true);
                    }
                    else
                    {
                        Console.WriteLine("File not found");
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Method to export the configuration files by weather station
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        public async Task<bool> exportCoordsWeatherStationAsync(string path, string mainCountry)
        {
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_WS_FILES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_WS_FILES);
            var weather_stations = await db.weatherStation.listEnableAsync();
            foreach (var ws in weather_stations.Where(p => p.visible && p.conf_files.Count() > 0))
            {
                var municipality = await db.municipality.byIdAsync(ws.municipality.ToString());
                var state = await db.state.byIdAsync(municipality.state.ToString());
                var country = await db.country.byIdAsync(state.country.ToString());
                if (country.id.ToString() == mainCountry)
                {
                    Console.WriteLine("Exporting coords ws: " + ws.name);
                    StringBuilder coords = new StringBuilder();
                    coords.Append("lat,lon\n");
                    coords.Append(ws.latitude.ToString() + "," + ws.longitude.ToString() + "\n");
                    File.WriteAllText(path + Program.settings.Out_PATH_WS_FILES + Path.DirectorySeparatorChar + ws.id.ToString() + "_coords.csv", coords.ToString());
                }
            }
            return true;
        }

        /// <summary>
        /// Method that exports the configuration for the forecast 
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        public async Task<bool> exportForecastSetupAsync(string path, string mainCountry)
        {
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_FS_FILES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_FS_FILES);
            var crops = await db.crop.listEnableAsync();
            foreach (var cp in crops)
            {
                Console.WriteLine("Exporting " + cp.name);
                string dir_crop = path + Program.settings.Out_PATH_FS_FILES + Path.DirectorySeparatorChar + Tools.folderCropName(cp.name);
                Directory.CreateDirectory(dir_crop);
                var setups = await db.setup.listEnableAsync();
                //var dir_def = "data_configuration/";
                foreach (var st in setups.Where(p => p.crop == cp.id))
                {
                    var weather_station = await db.weatherStation.byIdAsync(st.weather_station.ToString());
                    var municipality = await db.municipality.byIdAsync(weather_station.municipality.ToString());
                    var state = await db.state.byIdAsync(municipality.state.ToString());
                    var country = await db.country.byIdAsync(state.country.ToString());
                    if (country.id.ToString() == mainCountry)
                    {
                        string dir_setup = dir_crop + Path.DirectorySeparatorChar + st.weather_station.ToString() + "_" + st.cultivar.ToString() + "_" + st.soil.ToString() + "_" + st.days.ToString();
                        Directory.CreateDirectory(dir_setup);
                        foreach (var f in st.conf_files)
                        {
                            //File.Copy(dir_def + f.path.Substring(42), dir_setup + Path.DirectorySeparatorChar + f.name + COut.getExtension(f.path), true);
                            File.Copy(f.path, dir_setup + Path.DirectorySeparatorChar + f.name + COut.getExtension(f.path), true);
                        }
                        // Add csv file with geolocation for rice crop only
                        if (Program.settings.Out_CROPS_COORDINATES.Contains(Tools.folderCropName(cp.name)))
                        {
                            WeatherStation ws = await db.weatherStation.byIdAsync(st.weather_station.ToString());
                            StringBuilder coords = new StringBuilder();
                            coords.Append("name,value\n");
                            coords.Append("lat," + ws.latitude.ToString() + "\n");
                            coords.Append("long," + ws.longitude.ToString() + "\n");
                            coords.Append("elev," + ws.elevation.ToString() + "\n");
                            File.WriteAllText(dir_setup + Path.DirectorySeparatorChar + Program.settings.Out_PATH_FILE_COORDINATES, coords.ToString());
                        }

                        //  Export csv with season info to use in planting window
                        if (st.window)
                        {
                            
                            StringBuilder planting_window = new StringBuilder();
                            planting_window.Append("name,value\n");
                            planting_window.Append("window," + st.window.ToString() + "\n");
                            planting_window.Append("start_season," + st.season.start.ToString() + "\n");
                            planting_window.Append("end_season," + st.season.end.ToString() + "\n");
                            planting_window.Append("sowing_days," + st.season.sowing_days.ToString() + "\n");
                            File.WriteAllText(dir_setup + Path.DirectorySeparatorChar + Program.settings.Out_WINDOW_CONFIG, planting_window.ToString());
                        }

                        // Export csv with crop configuration
                        if (cp.crop_config != null && cp.crop_config.Count() > 0)
                        {

                            StringBuilder crop_config = new StringBuilder();
                            crop_config.Append("name,min,max,tag\n");
                            foreach (CropConfig c_config in cp.crop_config)
                            {
                                crop_config.Append(c_config.label.ToString() + ",");
                                crop_config.Append(c_config.min.ToString() + ",");
                                crop_config.Append(c_config.max.ToString() + ",");
                                crop_config.Append(c_config.type.ToString() + ",\n");
                            }
                                
                            File.WriteAllText(dir_setup + Path.DirectorySeparatorChar + Program.settings.Out_CROP_CONFIG, crop_config.ToString());
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Method that export all cpt  configuration needs by the forecast
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        public async Task<bool> exportCPTSetupAsync(string path, string mainCountry)
        {
            StringBuilder header_cpt, x_m, y_m, cca, gamma, header_areas;
            StringBuilder[] x1, x2, y1, y2;
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_STATES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES);
            var states = await db.state.listEnableAsync();
            IEnumerable<State> statesByCountry = states.Where(p => p.country.ToString() == mainCountry);
            // Filter the states with configuration
            var states_ctp = from s_cpt in statesByCountry
                             where s_cpt.conf.Where(p => p.track.enable).Count() > 0
                             select s_cpt;
            foreach (var s in states_ctp)
            {
                Console.WriteLine("Creating " + s.name);
                if (!Directory.Exists(path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString()))
                    Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString());

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
                    ConfigurationCPT conf = s.conf.LastOrDefault(p => p.trimester == (Quarter)Enum.Parse(typeof(Quarter), q) && p.track.enable);
                    bool config_exit = conf != null;
                    // the cpt configuration 
                    header_cpt.Append(q + ",");
                    x_m.Append(config_exit ? (conf.x_mode.ToString() ?? string.Empty) + "," : "NA,");
                    y_m.Append(config_exit ? conf.y_mode.ToString() + "," : "NA,");
                    cca.Append(config_exit ? conf.cca_mode.ToString() + "," : "NA,");
                    gamma.Append(config_exit ? conf.gamma.ToString() + "," : "NA,");
                    // the regions configurations
                    header_areas.Append(q + ",");
                    x1[0].Append(config_exit ? conf.regions.ElementAt(0).left_lower.lon.ToString() + "," : "NA,");
                    x2[0].Append(config_exit ? conf.regions.ElementAt(0).rigth_upper.lon.ToString() + "," : "NA,");
                    y1[0].Append(config_exit ? conf.regions.ElementAt(0).left_lower.lat.ToString() + "," : "NA,");
                    y2[0].Append(config_exit ? conf.regions.ElementAt(0).rigth_upper.lat.ToString() + "," : "NA,");

                    
                    // Second region
                    if (config_exit && conf.regions.Count() > 1)
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
                string file_name_cpt = path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString() + Path.DirectorySeparatorChar + "cpt" + ".csv";
                if (File.Exists(file_name_cpt))
                    File.Delete(file_name_cpt);
                File.WriteAllText(file_name_cpt, header_cpt.ToString().Substring(0, header_cpt.ToString().Length - 1) + "\n" +
                    x_m.ToString().Substring(0, x_m.ToString().Length - 1) + "\n" +
                    y_m.ToString().Substring(0, y_m.ToString().Length - 1) + "\n" +
                    cca.ToString().Substring(0, cca.ToString().Length - 1) + "\n" +
                    gamma.ToString().Substring(0, gamma.ToString().Length - 1));

                // Create the physical file regions
                string file_name_regions = path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString() + Path.DirectorySeparatorChar + "areas" + ".csv";
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
        /// Method to export the configuration files by weather station
        /// </summary>
        /// <param name="path">Path where the files will be located</param>
        public async Task<bool> exportUsersEmailsAsync(string path)
        {
            // Create directory
            if (!Directory.Exists(path + Program.settings.Out_PATH_USERS))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_USERS);
            var users = await db.user.listEnableBsonAsync();
            StringBuilder line = new StringBuilder();
            Console.WriteLine("Exporting users");
            foreach (var usr in users)
                line.Append(usr["Email"].ToString() + "\n");
            File.WriteAllText(path + Program.settings.Out_PATH_USERS + Path.DirectorySeparatorChar + "notify.csv", line.ToString());
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

        /// <summary>
        /// Method that calculates the periods for forecast when 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private List<PeriodsTgts> calculatePeriodsPyCPT(int m)
        {
            List<PeriodsTgts> r = new List<PeriodsTgts>();
            /*int i1 =  m == 1? 12 : ((m-1) % 13);
            int i2 = ((m + 1) % 13);
            int i3 = ((m + 2) % 13);
            int i4 = ((m + 4) % 13);
            r = new string[] { months[i1-1] + "-" + months[i2-1], months[i3-1] + "-" + months[i4-1] };
            #
            int i1 = m;
            int i2 = ((m + 2) % 13) == 0 ? 1 : ((m + 2) % 13);
            int i3 = ((m + 3) % 13) == 0 ? 1 : ((m + 3) % 13);
            int i4 = ((m + 5) % 13) == 0 ? 1 : ((m + 5) % 13);*/

            int i1 = m;
            int i2 = ((m + 2) > 12) ? (m + 2) - 12 : m + 2;
            int i3 = ((m + 3) > 12) ? (m + 3) - 12 : m + 3;
            int i4 = ((m + 5) > 12) ? (m + 5) - 12 : m + 5;

            string first_trimester_year = ((m + 1) > 12) ? DateTime.Now.AddYears(1).ToString("yyyy") : DateTime.Now.ToString("yyyy");
            string second_trimester_year = ((m + 4) > 12) ? DateTime.Now.AddYears(1).ToString("yyyy") : DateTime.Now.ToString("yyyy");


            PeriodsTgts first_trimester = new PeriodsTgts()
            {
                months = months[i1 - 1] + "-" + months[i2 - 1],
                year = first_trimester_year
            };

            PeriodsTgts second_trimester = new PeriodsTgts()
            {
                months = months[i3 - 1] + "-" + months[i4 - 1],
                year = second_trimester_year
            };

            r.Add(first_trimester);
            r.Add(second_trimester);
            return r;
        }

        /// <summary>
        /// Method that builds an entity that can be parse to json in order
        /// to export the configuration of PyCPT
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <param name="month_list"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> exportConfigurationPyCpt(string path, string id, List<int> month_list,TypePyCPT type )
        {
            Country country = await db.country.byIdAsync(id);
            var pycpt = type == TypePyCPT.seasonal ? country.conf_pycpt : country.subseasonal_pycpt;
            var conf_pycpt = pycpt.Where(p => p.track.enable == true).OrderByDescending(o => o.track.register);
            List<object> confs = new List<object>();
            foreach (var con in conf_pycpt.Where(p=> month_list.Contains(p.month)))
                confs.Add(new
                {
                    spatial_predictors = con.spatial_predictors.jsonConfiguration(),
                    spatial_predictands = con.spatial_predictands.jsonConfiguration(),
                    models = con.getModelsPyCPT(),
                    obs = ConfigurationPyCPT.getNameObs(con.obs),
                    station = con.station,
                    mos = ConfigurationPyCPT.getNameMos(con.mos),
                    predictand = ConfigurationPyCPT.getNamePredictand(con.predictand),
                    predictors = ConfigurationPyCPT.getNamePredictors(con.predictors),
                    mons = new string[] { months[con.month - 1], months[con.month - 1] },
                    //tgtii = new string[] { "1.5", "4.5" },
                    //tgtff = new string[] { "3.5", "6.5" },
                    tgtii = new string[] { "0.5", "3.5" },
                    tgtff = new string[] { "4.5", "6.5" },
                    tgts = calculatePeriodsPyCPT(con.month),
                    tini = con.ranges_years.min.ToString(),
                    tend = con.ranges_years.max.ToString(),
                    xmodes_min = con.xmodes.min.ToString(),
                    xmodes_max = con.xmodes.max.ToString(),
                    ymodes_min = con.ymodes.min.ToString(),
                    ymodes_max = con.ymodes.max.ToString(),
                    ccamodes_min = con.ccamodes.min.ToString(),
                    ccamodes_max = con.ymodes.max.ToString(),
                    force_download = con.force_download,
                    single_models = con.single_models,
                    forecast_anomaly = con.forecast_anomaly,
                    forecast_spi = con.forecast_spi,
                    confidence_level = con.confidence_level.ToString()
                });
            var jsn = JsonConvert.SerializeObject(confs);
            string full_path = path + Enum.GetName(typeof(TypePyCPT), type) + "_pycpt.json";
            if (File.Exists(full_path))
                File.Delete(full_path);
            File.WriteAllText(full_path, jsn);
            return true;
        }

        public async Task<bool> exportCoordsWsPycptAsync(string path, string mainCountry, string mainState = null)
        {
            // Create directory
            /*if (!Directory.Exists(path + Program.settings.Out_PATH_WSPYCPT_FILES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_WSPYCPT_FILES);*/
            var weather_stations = await db.weatherStation.listEnableAsync();
            if (mainCountry != null && mainState == null)
            {
                foreach (var ws in weather_stations.Where(p => p.visible))
                {
                    var municipality = await db.municipality.byIdAsync(ws.municipality.ToString());
                    var state = await db.state.byIdAsync(municipality.state.ToString());
                    var country = await db.country.byIdAsync(state.country.ToString());
                    if (country.id.ToString() == mainCountry)
                    {
                        Console.WriteLine("Exporting coords ws: " + ws.name);
                        StringBuilder coords = new StringBuilder();
                        if (!File.Exists(path + "stations_coords.csv"))
                        {
                            coords.Append("id,lat,lon\n");
                            coords.Append(ws.id.ToString() + "," + ws.latitude.ToString() + "," + ws.longitude.ToString() + "\n");
                            File.WriteAllText(path + "stations_coords.csv", coords.ToString());
                        }
                        else
                        {
                            coords.Append(ws.id.ToString() + "," + ws.latitude.ToString() + "," + ws.longitude.ToString() + "\n");
                            File.AppendAllText(path  + "stations_coords.csv", coords.ToString());
                        }
                    }
                }
            }
            else
            {
                var state = await db.state.byIdAsync(mainState);
                var country = await db.country.byIdAsync(state.country.ToString());
                var municipalities = await db.municipality.listEnableAsync();
                foreach (var municipality in municipalities.Where(p => p.visible && p.state == state.id))
                {
                    foreach (var ws in weather_stations.Where(q => q.visible && q.municipality == municipality.id))
                    {
                        Console.WriteLine("Exporting coords ws: " + ws.name);
                        StringBuilder coords = new StringBuilder();
                        if (!File.Exists(path + state.id.ToString() + Path.DirectorySeparatorChar + "stations_coords.csv"))
                        {
                            coords.Append("id,lat,lon\n");
                            coords.Append(ws.id.ToString() + "," + ws.latitude.ToString() + "," + ws.longitude.ToString() + "\n");
                            File.WriteAllText(path + state.id.ToString() + Path.DirectorySeparatorChar + "stations_coords.csv", coords.ToString());
                        }
                        else
                        {
                            coords.Append(ws.id.ToString() + "," + ws.latitude.ToString() + "," + ws.longitude.ToString() + "\n");
                            File.AppendAllText(path  + state.id.ToString() + Path.DirectorySeparatorChar + "stations_coords.csv", coords.ToString());
                        }
                    }
                }
            }
            return true;
        }

    }
}