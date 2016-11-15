using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the monthly climatological data from a weather station
    /// </summary>
    public class MonthlyDataStation
    {
        /// <summary>
        /// ID's weather station
        /// </summary>
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// Month of the year
        /// </summary>
        public int month { get; set; }
        /// <summary>
        /// Climatic data
        /// </summary>
        public IEnumerable<ClimaticData> data { get; set; }

    }
}
