using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about weather station.
    /// </summary>
    public class WeatherStation
    {
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// weather station's name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Id of the data source (external id)
        /// </summary>
        public string ext_id { get; set; }
        /// <summary>
        /// ID's municipality where weather station is located
        /// </summary>
        public ObjectId municipality { get; set; }
        /// <summary>
        /// Name of entity owns the station
        /// </summary>
        public string origin { get; set; }
        /// <summary>
        /// Decimal latitude of the location of the station
        /// </summary>
        public double latitude { get; set; }
        /// <summary>
        /// Decimal longitude of the location of the station
        /// </summary>
        public double longitude { get; set; }
        /// <summary>
        /// Array of configuration files
        /// </summary>
        public IEnumerable<ConfigurationFile> conf_files { get; set; }
        /// <summary>
        /// Array of yield ranges of the crops for the station
        /// </summary>
        public IEnumerable<YieldRange> ranges { get; set; }
        /// <summary>
        /// Indicates if the station is visible to queries via the web service
        /// </summary>
        public bool visible { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        public Track track { get; set; }
    }
}
