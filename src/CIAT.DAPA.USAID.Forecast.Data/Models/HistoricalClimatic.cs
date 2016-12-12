using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the monthly historical information on the climatic variables stations.
    /// </summary>
    public class HistoricalClimatic
    {
        /// <summary>
        /// ID's climatic historical
        /// </summary>
        [BsonId]
        [BsonRequired]
        public ObjectId id { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonRequired]
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// Year of the climatic data
        /// </summary>
        [BsonRequired]
        public int year { get; set; }
        /// <summary>
        /// Monthly data of the weather station
        /// </summary>
        [BsonRequired]
        public IEnumerable<MonthlyDataStation> monthly_data { get; set; }
    }
}
