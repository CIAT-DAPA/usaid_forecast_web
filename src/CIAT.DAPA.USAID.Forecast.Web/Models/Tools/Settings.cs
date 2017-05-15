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
    }
}
