using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the configuration that must be made at the time of generation of forecasts
    /// </summary>
    public partial class Setup
    {
        /// <summary>
        /// ID's setup
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonRequired]
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// ID's cultivar
        [BsonRequired]
        public ObjectId cultivar { get; set; }
        /// <summary>
        /// ID's soil
        /// </summary>
        [BsonRequired]
        public ObjectId soil { get; set; }
        /// <summary>
        /// Array of configuration files
        /// </summary>
        [BsonRequired]
        public IEnumerable<ConfigurationFile> conf_files { get; set; }
        /// <summary>
        /// Days of crop development
        /// </summary>
        [BsonRequired]
        public int days { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
        /// <summary>
        /// ID's crop
        /// </summary>
        [BsonRequired]
        public ObjectId crop { get; set; }

        /// <summary>
        /// Bool to decide if that setup use planting window
        /// </summary>
        public bool window { get; set; }

        /// <summary>
        /// Containt all information about the range of the planting window
        /// </summary>
        public Season season { get; set; }
    }
}
