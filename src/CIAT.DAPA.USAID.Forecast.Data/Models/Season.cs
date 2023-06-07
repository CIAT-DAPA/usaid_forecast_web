using MongoDB.Bson;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the ranges of the planting window is active
    /// </summary>
    public class Season
    {
        /// <summary>
        /// Month when planting window start
        /// </summary>
        public int start { get; set; }
        /// <summary>
        /// Month when planting window finish
        /// </summary>
        public int end { get; set; }
        /// <summary>
        /// Sowing days it's a env equal to 45
        /// </summary>
        public int sowing_days { get; set; }
    }
}
