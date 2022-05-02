using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about the countries which users have permission.
    /// </summary>
    public partial class UserPermission
    {
        /// <summary>
        /// ID's user country
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        [BsonRequired]
        public string user { get; set; }

        /// <summary>
        /// Contains the countries which user can manage
        /// </summary>
        [BsonRequired]
        public IEnumerable<ObjectId> countries { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }

    }
}
