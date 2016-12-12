using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
        /// Month of the year
        /// </summary>
        [BsonRequired]
        public int month { get; set; }
        /// <summary>
        /// Climatic data
        /// </summary>
        [BsonRequired]
        public IEnumerable<ClimaticData> data { get; set; }

    }
}
