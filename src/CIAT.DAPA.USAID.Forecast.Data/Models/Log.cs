using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the activities record over platform
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Date event
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// User that executed the event
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// Event's name
        /// </summary>
        [BsonRepresentation(BsonType.String)]        
        public LogEvent type_event { get; set; }
        /// <summary>
        /// List of entities affected in the event
        /// </summary>
        [BsonRepresentation(BsonType.Array)]
        public IEnumerable<LogEntity> entities { get; set; }
        /// <summary>
        /// Description's event
        /// </summary>
        public string content { get; set; }
    }
}
