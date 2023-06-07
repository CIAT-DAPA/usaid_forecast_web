using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Tools
{
    /// <summary>
    /// This class load the settings of the app from the appsettings.json
    /// </summary>
    public class Settings
    {
        public char splitted { get; set; }
        /// <summary>
        /// String to connect with the database
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Name of the database
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// Name of directory to export the data of the states historical climate
        /// </summary>
        public string Out_PATH_STATES { get; set; }
        /// <summary>
        /// Name of directory to export the configuration files of weather stations
        /// </summary>
        public string Out_PATH_WS_FILES { get; set; }
        public string Out_PATH_WSPYCPT_FILES { get; set; }
        /// <summary>
        /// Name of directory to export the setup for the yield forecast
        /// </summary>
        public string Out_PATH_FS_FILES { get; set; }
        /// <summary>
        /// Name of directory to export the emails users
        /// </summary>
        public string Out_PATH_USERS { get; set; }
        /// <summary>
        /// Arrays with the crops list that neeed the coordinates file
        /// </summary>
        public string[] Out_CROPS_COORDINATES { get; set; }
        /// <summary>
        /// Name of file to export the coordinates of the weather stations
        /// </summary>
        public string Out_PATH_FILE_COORDINATES { get; set; }
        /// <summary>
        /// Name of directory to import the probabilities of the forecast
        /// </summary>
        public string In_PATH_FS_PROBABILITIES { get; set; }
        /// <summary>
        /// Name of directory to get the raster files of the forecast
        /// </summary>
        public string In_PATH_FS_RASTER_SOURCE { get; set; }
        /// <summary>
        /// Name of directory to copy the raster files of the forecast
        /// </summary>
        public string In_PATH_FS_RASTER_DESTINATION { get; set; }
        /// <summary>
        /// File's name of the forecast probabilities
        /// </summary>
        public string In_PATH_FS_FILE_PROBABILITY { get; set; }
        /// <summary>
        /// File's name of the forecast probabilities for subseasonal
        /// </summary>
        public string In_PATH_FS_FILE_SUBSEASONAL { get; set; }
        /// <summary>
        /// File's name of the performance probabilities
        /// </summary>
        public string In_PATH_FS_FILE_PERFORMANCE { get; set; }
        /// <summary>
        /// Name of directory to import the scenarios of the forecast
        /// </summary>
        public string In_PATH_FS_SCENARIOS { get; set; }
        /// <summary>
        /// Name of directory to import a scenario
        /// </summary>
        public string In_PATH_FS_D_SCENARIO { get; set; }
        /// <summary>
        /// Name of directory to import the yield of the forecast
        /// </summary>
        public string In_PATH_FS_YIELD { get; set; }
        /// <summary>
        /// Name of directory to import the climate of the forecast
        /// </summary>
        public string In_PATH_FS_CLIMATE { get; set; }
        /// <summary>
        /// Message to share in the social networks
        /// </summary>
        public string Social_Network_Message { get; set; }
        /// <summary>
        /// Consumer key of the twitter
        /// </summary>
        public string Social_Network_Twitter_ConsumerKey { get; set; }
        /// <summary>
        /// Consumer Key Secret of the twitter
        /// </summary>
        public string Social_Network_Twitter_ConsumerKeySecret { get; set; }
        /// <summary>
        /// Access Token of the twitter
        /// </summary>
        public string Social_Network_Twitter_AccessToken { get; set; }
        /// <summary>
        /// Access Token Secret of the twitter
        /// </summary>
        public string Social_Network_Twitter_AccessTokenSecret { get; set; }
        /// <summary>
        /// Set or get if it should add one day for crop forecast
        /// </summary>
        public bool Add_Day { get; set; }
        /// <summary>
        /// Set or get the path where the configuration folder of webadmin is
        /// </summary>
        public string In_PATH_D_WEBADMIN_CONFIGURATION { get; set; }
        /// <summary>
        /// Set or get the default value of sowing days
        /// </summary>
        public int SOWING_DAYS { get; set; }
        /// <summary>
        /// Set or get the name of the window config file
        /// </summary>
        public string Out_WINDOW_CONFIG { get; set; }
        /// <summary>
        /// Set or get the name of the crop config file
        /// </summary>
        public string Out_CROP_CONFIG { get; set; }
        /// <summary>
        /// Set or get the name of the phenological phase config file
        /// </summary>
        public string In_PATH_FS_FILE_PHENO_PHASES { get; set; }

    }
}
