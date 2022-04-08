using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the configuration that must be made at the time of generation of climate forecasts with pycpt
    /// </summary>
    public partial class ConfigurationPyCPT
    {
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public SpatialCoords spatial_predictors { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public SpatialCoords spatial_predictands { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public IEnumerable<ModelsPyCpt> models { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public Obs obs { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public Boolean station { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public Mos mos { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public Predictand predictand { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public Predictors predictors { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public IEnumerable<Mons> mons { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public IEnumerable<String> tgtii { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public IEnumerable<String> tgtff { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public IEnumerable<Quarter> tgts { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int tini { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int tend { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int xmodes_min { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int xmodes_max { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int ymodes_min { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int ymodes_max { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int ccamodes_min { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int ccamodes_max { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public Boolean force_download { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public Boolean single_models { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public Boolean forecast_anomaly { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public Boolean forecast_spi { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public int confidence_level { get; set; }
        /// <summary>
        /// Number that indicate the execution 0= country and 1= state/region
        /// </summary>
        [BsonRequired]
        public int ind_exec { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
    }
}
