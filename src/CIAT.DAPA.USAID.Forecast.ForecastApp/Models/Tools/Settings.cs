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
        /// File's name of the forecast probabilities
        /// </summary>
        public string In_PATH_FS_FILE_PROBABILITY { get; set; }
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
        /// Name of directory to import the climate of the forecast
        /// </summary>
        public string Social_Network_Message { get; set; }
    }
}
