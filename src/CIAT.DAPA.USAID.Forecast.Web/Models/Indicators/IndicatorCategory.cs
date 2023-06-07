using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Indicators
{
    /// <summary>
    /// Class for Cualitative Indicators
    /// </summary>
    public class IndicatorCategory
    {
        /// <summary>
        /// Get or set the Id of category
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Get or set the description of category
        /// </summary>
        public string Description { get; set; }
    }
}
