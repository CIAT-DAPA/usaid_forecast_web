using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about municipalities.
    /// </summary>
    public partial class WeatherStationData
    {
        /// <summary>
        /// ID's municipality
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name of the department or state in which is located the locality
        /// </summary>
        [BsonRequired]
        public string name { get; set; }

        public ObjectId depends { get; set; }
    }
}
