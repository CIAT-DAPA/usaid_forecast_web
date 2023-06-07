using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about weather station.
    /// </summary>
    public class WeatherStationAllData
    {
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// weather station's name
        /// </summary>
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// Id of the data source (external id)
        /// </summary>
        [BsonRequired]
        public string ext_id { get; set; }
        /// <summary>
        /// Name of entity owns the station
        /// </summary>
        [BsonRequired]
        public string origin { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        /// <summary>
        /// Array of yield ranges of the crops for the station
        /// </summary>        
        [BsonRequired]
        public IEnumerable<YieldRange> ranges { get; set; }

        public IEnumerable<WeatherStationData> munc { get; set; }

        public IEnumerable<WeatherStationData> std { get; set; }
    }
}
