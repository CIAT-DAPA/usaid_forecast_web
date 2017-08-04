using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the configuration that must be made at the time of generation of climate forecasts with cpt
    /// </summary>
    public partial class ConfigurationCPT
    {
        /// <summary>
        /// Sets the quarter for which the forecast is made
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public Quarter trimester { get; set; }
        /// <summary>
        /// List of theoretical regions
        /// </summary>
        [BsonRequired]
        public IEnumerable<Region> regions { get; set; }
        /// <summary>
        /// Number of modes in x
        /// </summary>
        [BsonRequired]
        public int x_mode { get; set; }
        /// <summary>
        /// Number of modes in y
        /// </summary>
        [BsonRequired]
        public int y_mode { get; set; }
        /// <summary>
        /// Number of modes in canonical correlation
        /// </summary>
        [BsonRequired]
        public int cca_mode { get; set; }
        /// <summary>
        /// Sets if the gamma transformation is used
        /// </summary>
        [BsonRequired]
        public bool gamma { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
    }
}
