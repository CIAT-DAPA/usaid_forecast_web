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
        /// Geogrpahic path of the Web Api forecast
        /// </summary>
        public string api_fs_geographic { get; set; }
        /// <summary>
        /// Agronomic path of the Web Api forecast
        /// </summary>
        public string api_fs_agronomic { get; set; }
        /// <summary>
        /// Forecast path of the Web Api forecast climate
        /// </summary>
        public string api_fs_forecast_climate{ get; set; }
        /// <summary>
        /// Forecast path of the Web Api forecast yield
        /// </summary>
        public string api_fs_forecast_yield { get; set; }
        /// <summary>
        /// Historical path of the Web Api climate historical
        /// </summary>
        public string api_fs_historical { get; set; }
        /// <summary>
        /// Historical path of the Web Api yield historical years
        /// </summary>
        public string api_fs_historical_yield_years { get; set; }
        /// <summary>
        /// Historical path of the Web Api yield historical
        /// </summary>
        public string api_fs_historical_yield { get; set; }
    }
}
