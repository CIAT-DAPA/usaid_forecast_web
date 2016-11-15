using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about the states or departments.
    /// </summary>
    public class State
    {
        /// <summary>
        /// ID's state
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name of the department or state in which is located the locality
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Country in which is located the state
        /// </summary>
        public Country country { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        public Track track { get; set; }
    }
}
