using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about the historical of crop yield
    /// </summary>
    public class HistoricalYield
    {
        /// <summary>
        /// ID's of register of the historical
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name where the historical data is obtained
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// Data of the pronostic for the crop
        /// </summary>
        public IEnumerable<YieldCrop> yield { get; set; }
    }
}
