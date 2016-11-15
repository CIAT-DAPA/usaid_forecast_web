using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about municipalities.
    /// </summary>
    public class Municipality
    {
        /// <summary>
        /// ID's municipality
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name of the department or state in which is located the locality
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// ID's state where municipality is located
        /// </summary>
        public ObjectId state { get; set; }        
        /// <summary>
        /// Indicates if the entity is visible to queries via the web service
        /// </summary>
        public bool visible { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        public Track track { get; set; }
    }
}
