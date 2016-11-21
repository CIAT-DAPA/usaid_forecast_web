using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information on activities in the site of administration
    /// </summary>
    public class LogAdministrative
    {
        /// <summary>
        /// ID's event in the administrative application
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Event data
        /// </summary>
        [BsonRequired]
        public Log data { get; set; }
    }
}
