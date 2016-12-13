using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the weather (historical average) climatic variables weather stations.
    /// </summary>
    public class Climatology
    {
        /// <summary>
        /// ID's climatology
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonRequired]
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// Monthly data of the weather station
        /// </summary>
        [BsonRequired]
        public IEnumerable<MonthlyDataStation> monthly_data { get; set; }
    }
}
