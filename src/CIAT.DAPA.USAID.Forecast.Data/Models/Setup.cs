using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the configuration that must be made at the time of generation of forecasts
    /// </summary>
    public class Setup
    {
        /// <summary>
        /// ID's weather station
        /// </summary>
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// ID's cultivar
        /// </summary>
        public ObjectId cultivar { get; set; }
        /// <summary>
        /// ID's soil
        /// </summary>
        public ObjectId soil { get; set; }
        /// <summary>
        /// Array of configuration files
        /// </summary>
        public IEnumerable<ConfigurationFile> conf_files { get; set; }
        /// <summary>
        /// Days of crop development
        /// </summary>
        public int days { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        public Track track { get; set; }
    }
}
