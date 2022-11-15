using MongoDB.Bson;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the yield ranges of the crops in the weather station
    /// </summary>
    public class Season
    {
        /// <summary>
        /// Label for the range
        /// </summary>
        public DateTime start { get; set; }
        /// <summary>
        /// Lower limit
        /// </summary>
        public DateTime end { get; set; }
        /// <summary>
        /// Upper limit
        /// </summary>
        public int sowing_days { get; set; }
    }
}
