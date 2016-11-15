using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information on activities in the web service
    /// </summary>
    public class LogService
    {
        /// <summary>
        /// ID's event in the service
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Event data
        /// </summary>
        public Log data { get; set; }
    }
}
