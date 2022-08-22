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
        /// <summary>
        /// Get or set the geoser url
        /// </summary>
        public string indicator_geoserver_url { get; set; }
        /// <summary>
        /// Get or set the workspace name into geoserver where indicators are storage
        /// </summary>
        public string indicator_geoserver_workspace { get; set; }
        /// <summary>
        /// Get or set the id for layer which has the average of indicators
        /// </summary>
        public int indicator_geoserver_average { get; set; }
        /// <summary>
        /// Get or set the id for layer which has the coefitien of variation of indicators
        /// </summary>
        public int indicator_geoserver_cv { get; set; }
        /// <summary>
        /// Get or set the period of time of time series mosaic
        /// </summary>
        public int[] indicator_geoserver_time { get; set; }
        /// <summary>
        /// Get or set the year where NINO data is saved in the time series of mosaic (geoserver)
        /// </summary>
        public int indicator_NINO { get; set; }
        /// <summary>
        /// Get or set the year where NINA data is saved in the time series of mosaic (geoserver)
        /// </summary>
        public int indicator_NINA { get; set; }
    }
}
