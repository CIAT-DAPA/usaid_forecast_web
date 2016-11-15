using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

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
        public ObjectId id { get; set; }
        /// <summary>
        /// Year of the climatic data
        /// </summary>
        public int year { get; set; }
        /// <summary>
        /// Monthly data of the weather station
        /// </summary>
        public MonthlyDataStation monthly_data { get; set; }
    }
}
