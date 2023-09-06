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
using Newtonsoft.Json.Linq;

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
            //Get current month
            DateTime CurrentDate = DateTime.Now;
            int currentMonth = CurrentDate.Month == 12 ? 0 : CurrentDate.Month;
      
            //Create folders
            if (!Directory.Exists(path + Program.settings.Out_PATH_STATES))
                Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES);
            List<State> states = await db.state.listEnableAsync();
            IEnumerable<State> statesByCountry = states.Where(p => p.country.ToString() == mainCountry);
            // Filter the states with configuration
            IEnumerable<State> states_ctp = from s_cpt in statesByCountry
                             where s_cpt.conf.Where(p => p.track.enable).Count() > 0
                             select s_cpt;
            foreach (State s in states_ctp)
            {
                List<object> cpt_info = new List<object>();
                Console.WriteLine("Creating " + s.name);
                if (!Directory.Exists(path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString()))
                    Directory.CreateDirectory(path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString());
                ForecastType type = s.conf.FirstOrDefault(p => p.track.enable).forc_type;
                List<ConfigurationCPT> listOfConfig;             
                if(type.ToString() == "tri")
                {
                    Quarter current_quarter = ((Quarter)currentMonth);
                    listOfConfig = s.conf.Where(p => (p.trimester == current_quarter || p.trimester == GetQuarter(current_quarter)) && p.track.enable)
                        .OrderBy(p => GetCustomOrder(p.trimester, currentMonth, type)).ToList();

                    foreach (ConfigurationCPT config in listOfConfig)
                    {

                        cpt_info.Add(new
                        {
                            type = type.ToString(),
                            season = changeSeason(type.ToString(), (int)config.trimester),
                            areas = getRegions(config),
                            modes = new ModesCpt() { 
                                x = config.x_mode.ToString(),
                                y = config.y_mode.ToString(),
                                cca = config.cca_mode.ToString()
                            },
                            transformation = getTransformation(config),
                            predictand = config.predictand.ToString(),

                        });
                    }
                    cpt_info = fillListOfConfig(listOfConfig, s, current_quarter, cpt_info, type);
                }
                else if(type.ToString() == "bi")
                {
                    Quarter current_quarter = ((Quarter)currentMonth + 12);
                    listOfConfig = s.conf.Where(p => (p.trimester == current_quarter || p.trimester == GetBimonthly(current_quarter,1) || p.trimester == GetBimonthly(current_quarter,2)) && p.track.enable)
                        .OrderBy(p => GetCustomOrder(p.trimester, currentMonth, type)).ToList();
                    foreach (ConfigurationCPT config in listOfConfig)
                    {

                        cpt_info.Add(new
                        {
                            type = type.ToString(),
                            season = changeSeason(type.ToString(), (int)config.trimester),
                            areas = getRegions(config),
                            modes = new ModesCpt()
                            {
                                x = config.x_mode.ToString(),
                                y = config.y_mode.ToString(),
                                cca = config.cca_mode.ToString()
                            },
                            transformation = getTransformation(config),
                            predictand = config.predictand.ToString(),

                        });
                    }
                    cpt_info = fillListOfConfig(listOfConfig, s, current_quarter, cpt_info, type);
                }

                String jsn = JsonConvert.SerializeObject(cpt_info);
                string full_path = path + Program.settings.Out_PATH_STATES + Path.DirectorySeparatorChar + s.id.ToString() + Path.DirectorySeparatorChar + Program.settings.Out_PATH_CPT_FILE;
                if (File.Exists(full_path))
                    File.Delete(full_path);
                File.WriteAllText(full_path, jsn);

            }
            return true;
        }

        public static string changeSeason(string type, int season)
        {
            string season_result = "";
            if(type == "tri")
            {
                string[] season_names = new string [] {
                    "Dec-Jan-Feb",
                    "Jan-Feb-Mar",
                    "Feb-Mar-Apr",
                    "Mar-Apr-May",
                    "Apr-May-Jun",
                    "May-Jun-Jul",
                    "Jun-Jul-Aug",
                    "Jul-Aug-Sep",
                    "Aug-Sep-Oct",
                    "Sep-Oct-Nov",
                    "Oct-Nov-Dec",
                    "Nov-Dec-Jan"
                };
                season_result = season_names[season];
            }
            else
            {
                season = season - 12;
                string[] season_names = new string[]
                {
                    "Dec-Jan",
                    "Jan-Feb",
                    "Feb-Mar",
                    "Mar-Apr",
                    "Apr-May",
                    "May-Jun",
                    "Jun-Jul",
                    "Jul-Aug",
                    "Aug-Sep",
                    "Sep-Oct",
                    "Oct-Nov",
                    "Nov-Dec"
                };
                season_result = season_names[season];
            }
            return season_result;
        }

        public static List<object> fillListOfConfig(List<ConfigurationCPT> listOfConfig, State s, Quarter current_quarter, List<object> cpt_info, ForecastType type)
        {
            List<object> newList = cpt_info;
            if(type.ToString() == "tri")
            {
                if (listOfConfig.Count() == 2)
                {
                    return newList;
                }
                if (listOfConfig.Count() == 1)
                {
                    if (listOfConfig.FirstOrDefault().trimester == current_quarter)
                    {
                        newList.Add(new
                        {
                            type = type.ToString(),
                            season = GetQuarter(current_quarter).ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });
                    }
                    else
                    {
                        newList.Add(new
                        {
                            type = type.ToString(),
                            season = current_quarter.ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });
                    }
                }
                else if (listOfConfig.Count() == 0)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        newList.Add(new
                        {
                            type = type.ToString(),
                            season = x == 0 ? current_quarter.ToString() : GetQuarter(current_quarter).ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });
                    }
                }
            }
            else
            {
                if(listOfConfig.Count() == 3)
                {
                    return newList;
                }
                else if(listOfConfig.Count() == 2)
                {
                    if (listOfConfig.FirstOrDefault().trimester == current_quarter)
                    {
                        if(listOfConfig[1].trimester == GetBimonthly(current_quarter, 1))
                        {
                            newList.Add(new
                            {
                                type = type.ToString(),
                                season = GetBimonthly(current_quarter,2).ToString(),
                                areas = new JArray(),
                                modes = new JObject(),
                                transformation = new JArray(),
                                predictand = "",

                            });
                        }
                        else
                        {
                            newList.Insert(1,new
                            {
                                type = type.ToString(),
                                season = GetBimonthly(current_quarter, 1).ToString(),
                                areas = new JArray(),
                                modes = new JObject(),
                                transformation = new JArray(),
                                predictand = "",

                            });
                        }
                        
                    }else if(listOfConfig.FirstOrDefault().trimester == GetBimonthly(current_quarter, 1))
                    {

                            newList.Insert(0,new
                            {
                                type = type.ToString(),
                                season = current_quarter.ToString(),
                                areas = new JArray(),
                                modes = new JObject(),
                                transformation = new JArray(),
                                predictand = "",

                            });

                    }
                    
                }
                else if (listOfConfig.Count() == 1)
                {
                    if (listOfConfig.FirstOrDefault().trimester == current_quarter)
                    {

                        newList.Add(new
                        {
                            type = type.ToString(),
                            season = GetBimonthly(current_quarter, 1).ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });
                        newList.Add(new
                        {
                            type = type.ToString(),
                            season = GetBimonthly(current_quarter, 2).ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });



                    }
                    else if (listOfConfig.FirstOrDefault().trimester == GetBimonthly(current_quarter, 1))
                    {

                        newList.Insert(0, new
                        {
                            type = type.ToString(),
                            season = current_quarter.ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });
                        newList.Insert(2, new
                        {
                            type = type.ToString(),
                            season = GetBimonthly(current_quarter, 2).ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });

                    }
                    else if (listOfConfig.FirstOrDefault().trimester == GetBimonthly(current_quarter, 2))
                    {

                        newList.Insert(0, new
                        {
                            type = type.ToString(),
                            season = current_quarter.ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });
                        newList.Insert(1, new
                        {
                            type = type.ToString(),
                            season = GetBimonthly(current_quarter, 1).ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });

                    }
                }
                else if (listOfConfig.Count() == 0)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        newList.Add(new
                        {
                            type = type.ToString(),
                            season = x == 0 ? current_quarter.ToString() : x == 1 ? GetBimonthly(current_quarter,1).ToString()  : GetBimonthly(current_quarter,2).ToString(),
                            areas = new JArray(),
                            modes = new JObject(),
                            transformation = new JArray(),
                            predictand = "",

                        });
                    }
                }
            }
            
            return newList;
        }

        public static List<AreasCpt> getRegions(ConfigurationCPT config)
        {
            List<AreasCpt> r = new List<AreasCpt>();
            foreach(Region region in config.regions)
            {
                AreasCpt area = new AreasCpt()
                {
                    predictor = region.predictor.ToString(),
                    x_min = region.left_lower.lon.ToString(),
                    x_max = region.rigth_upper.lon.ToString(),
                    y_min = region.left_lower.lat.ToString(),
                    y_max = region.rigth_upper.lat.ToString(),
                };
                r.Add(area);
            }
            //Add NA region object if this part is uncomment
            /*if(r.Count() == 1)
            {
                r.Add(new AreasCpt()
                {
                    x_min = "NA",
                    x_max = "NA",
                    y_min = "NA",
                    y_max = "NA",
                });
            }*/
            
            return r;
        }

        public static List<TransformationCpt> getTransformation(ConfigurationCPT config)
        {
            List<TransformationCpt> t = new List<TransformationCpt>();
            TransformationCpt transformation = new TransformationCpt()
            {
                gamma = config.gamma
            };
            t.Add(transformation);
            return t;
        }

        public static Quarter GetQuarter(Quarter current_quarter)
        {
            Quarter quarter;
            if(Convert.ToInt32(current_quarter) > 8)
            {
                int value = Convert.ToInt32(current_quarter) - 9;
                quarter = ((Quarter)value);
            }
            else
            {
                quarter = current_quarter + 3;
            }
            return quarter;
        }

        public static Quarter GetBimonthly(Quarter current_quarter, int number)
        {
            Quarter quarter;
            if (Convert.ToInt32(current_quarter) >= 22)
            {
                if (number == 1)
                {
                    int value = Convert.ToInt32(current_quarter) - 10;
                    quarter = ((Quarter)value);
                }
                else
                {
                    int value = Convert.ToInt32(current_quarter) - 8;
                    quarter = ((Quarter)value);
                }

            }
            else if (Convert.ToInt32(current_quarter) >= 20)
            {
                if(number == 1)
                {
                    quarter = current_quarter + 2;
                }
                else {
                    int value = Convert.ToInt32(current_quarter) - 8;
                    quarter = ((Quarter)value);
                }

            }
            else
            {
                if (number == 1)
                {
                    quarter = current_quarter + 2;
                }
                else
                {
                    quarter = current_quarter + 4;
                }
                
            }
            return quarter;
        }
        private static int GetCustomOrder(Quarter trimester, int currentMonth, ForecastType type)
        {
            int trimesterValue = (int)trimester;

            if (type == ForecastType.tri)
            {
                // Ajustar el valor del trimestre si es mayor que el mes actual
                if (trimesterValue < currentMonth)
                {
                    trimesterValue += 12;
                }
            }
            else if (type == ForecastType.bi)
            {
                // Ajustar el valor del bimestre si es mayor que el mes actual
                if (trimesterValue < currentMonth + 12)
                {
                    trimesterValue += 12;
                }
            }

            return trimesterValue;
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


        public async Task<bool> exportUrls(string path, string country, string type)
        {
            UrlsJsonFormat json_data = new UrlsJsonFormat()
            {
                status = false,
                msg = "Error",
            };
            string full_path = path + Path.DirectorySeparatorChar + country + "_" + type + ".json";
            try
            {
                IEnumerable<Url> url = await db.url.byIndexAsync(MongoDB.Bson.ObjectId.Parse(country), type);
                if(url.Count() != 1)
                {
                    json_data.msg = "Url not found in the databaase";
                    String jsn_error = JsonConvert.SerializeObject(json_data);
                    
                    if (File.Exists(full_path))
                        File.Delete(full_path);
                    File.WriteAllText(full_path, jsn_error);
                    return false;
                }
                Url selected_url = url.FirstOrDefault();
                if(selected_url.type == UrlTypes.download_seasonal_prec)
                {
                    json_data.status = true;
                    json_data.msg = "ok";
                    json_data.urls = new List<UrlJsonData>();
                    foreach (UrlData data in selected_url.urls)
                    {
                        int INITIAL_MONTH_VALUE_GUATE = 755;
                        int INITIAL_YEAR_VALUE_GUATE = 2023;
                        string url_value = data.value.Replace("{month}", Convert.ToString(DateTime.Now.Month) + ".0");
                        int month = ((DateTime.Now.Year - INITIAL_YEAR_VALUE_GUATE) *12) + (INITIAL_MONTH_VALUE_GUATE + DateTime.Now.Month);
                        url_value = url_value.Replace("{g_month}", Convert.ToString(month) + ".0");
                        url_value = url_value.Replace("{category}", data.prob_type.ToString());
                        url_value = url_value.Replace("{g_category}", getCategoryGuate(data.prob_type.ToString()));
                        url_value = url_value.Replace("{file_name}", data.name.ToString());
                        json_data.urls.Add(new UrlJsonData()
                        {
                            name = data.name,
                            value = url_value
                        });
                    }
                }
                String jsn = JsonConvert.SerializeObject(json_data);
                if (File.Exists(full_path))
                    File.Delete(full_path);
                File.WriteAllText(full_path, jsn);
                return true;
            }
            catch(Exception ex)
            {
                json_data.msg = ex.Message;
                String jsn = JsonConvert.SerializeObject(json_data);
                if (File.Exists(full_path))
                    File.Delete(full_path);
                File.WriteAllText(full_path, jsn);
                return false;
            }
            
        }

        public static string getCategoryGuate(string category)
        {
            string translatedValue = category;

            switch (category)
            {
                case "above":
                    translatedValue = "Sobre";
                    break;
                case "normal":
                    translatedValue = "Normal";
                    break;
                case "below":
                    translatedValue = "Bajo";
                    break;
                case "deterministic":
                    translatedValue = "Determinístico";
                    break;
            }
            return translatedValue;
        }
    }
}