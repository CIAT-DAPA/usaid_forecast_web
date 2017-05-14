using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools
{
    /// <summary>
    /// This class load the settings of the web API from the appsettings.json
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
        /// Path where located the log
        /// </summary>
        public string LogPath { get; set; }
        /// <summary>
        /// Symbol to delimiter export 
        /// </summary>
        public string Delimiter { get; set; }
    }
}
