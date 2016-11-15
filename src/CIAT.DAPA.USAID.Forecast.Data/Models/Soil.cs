using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about different soil types
    /// </summary>
    public class Soil
    {
        /// <summary>
        /// ID's soil
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Soil's name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// ID's owner crop
        /// </summary>
        public ObjectId crop { get; set; }
        /// <summary>
        /// Indicates the order they should be listed
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// Array of configuration files
        /// </summary>
        public IEnumerable<ConfigurationFile> conf_files { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        public Track track { get; set; }
    }
}
