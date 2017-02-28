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
    }
}
