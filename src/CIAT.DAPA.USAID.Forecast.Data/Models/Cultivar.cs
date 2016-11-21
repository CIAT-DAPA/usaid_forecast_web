using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about different cultivar of the crop
    /// </summary>
    public class Cultivar
    {
        /// <summary>
        /// ID's cultivar
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Cultivar's name
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
        /// Sets whether or not the variety is dry. In case it is not, 
        /// it is assumed that it is irrigated
        /// </summary>
        public bool rainfed { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        public Track track { get; set; }
    }
}
