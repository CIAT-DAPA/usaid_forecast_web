using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

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
        /// Data of the pronostic for the crop
        /// </summary>
        public YieldCrop yield { get; set; }
    }
}
