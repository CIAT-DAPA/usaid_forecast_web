using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the activities record over platform
    /// </summary>
    public partial class Log
    {
        /// <summary>
        /// Date event
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime date { get; set; }
        /// <summary>
        /// User that executed the event
        /// </summary>
        [BsonRequired]
        public string user { get; set; }
        /// <summary>
        /// Event's name
        /// </summary>
        [BsonRequired]
        [BsonRepresentation(BsonType.String)]        
        public LogEvent type_event { get; set; }
        /// <summary>
        /// List of entities affected in the event
        /// </summary>
        [BsonRequired]
        [BsonRepresentation(BsonType.String)]
        public IEnumerable<LogEntity> entities { get; set; }
        /// <summary>
        /// Description's event
        /// </summary>
        [BsonRequired]
        public string content { get; set; }
    }
}
