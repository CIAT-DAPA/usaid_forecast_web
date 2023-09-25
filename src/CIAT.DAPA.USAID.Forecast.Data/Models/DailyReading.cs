using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the daily data on the climatic variables stations.
    /// </summary>
    public class DailyReading
    {

        /// <summary>
        /// day of the daily climatic data
        /// </summary>
        [BsonRequired]
        public int day { get; set; }
        /// <summary>
        /// Climatic data
        /// </summary>
        [BsonRequired]
        public IEnumerable<ClimaticData> data { get; set; }

    }
}
