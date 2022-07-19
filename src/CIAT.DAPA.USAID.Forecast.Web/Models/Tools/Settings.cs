using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Tools
{
    /// <summary>
    /// This class load the settings of the web site from the appsettings.json
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Root path of the Web Api forecast
        /// </summary>
        public string api_fs { get; set; }
        /// <summary>
        /// Country id
        /// </summary>
        public string idCountry { get; set; }
        /// <summary>
        /// Get or set if indicators module is enable
        /// </summary>
        public bool modules_indicators { get; set; }
        /// <summary>
        /// Get or set if rice module is enable
        /// </summary>
        public bool modules_rice { get; set; }
        /// <summary>
        /// Get or set if rice module is enable
        /// </summary>
        public bool modules_maize { get; set; }

        public string indicator_geoserver_url { get; set; }
        public string indicator_geoserver_workspace { get; set; }
        public int indicator_geoserver_average { get; set; }
        public int[] indicator_geoserver_time { get; set; }
    }
}
