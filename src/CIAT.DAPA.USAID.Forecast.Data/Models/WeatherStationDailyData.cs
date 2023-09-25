using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the daily data on the climatic variables stations.
    /// </summary>
    public class WeatherStationDailyData
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
        /// Month of the daily climatic data
        /// </summary>
        [BsonRequired]
        public int month { get; set; }
        /// <summary>
        /// Year of the daily climatic data
        /// </summary>
        [BsonRequired]
        public int year { get; set; }
        /// <summary>
        /// Daily data
        /// </summary>
        [BsonRequired]
        public IEnumerable<DailyReading> daily_readings { get; set; }

    }
}
