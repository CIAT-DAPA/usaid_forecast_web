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
        /// Define the spatial region for predictors
        /// </summary>
        [BsonRequired]
        public Region spatial_predictors { get; set; }
        /// <summary>
        /// Define the spatial region for predictands
        /// </summary>
        [BsonRequired]
        public Region spatial_predictands { get; set; }
        /// <summary>
        /// List of models that will include in the forecast process
        /// </summary>
        [BsonRequired]
        [BsonRepresentation(BsonType.String)]
        public IEnumerable<ModelsPyCpt> models { get; set; }
        /// <summary>
        /// Type of observational data
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public Obs obs { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public bool station { get; set; }
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
        public int month { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public RangeParameter ranges_years { get; set; }        
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public RangeParameter xmodes { get; set; }        
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public RangeParameter ymodes { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public RangeParameter ccamodes { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public bool force_download { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public bool single_models { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public bool forecast_anomaly { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public bool forecast_spi { get; set; }
        /// <summary>
        ///
        /// </summary>
        [BsonRequired]
        public double confidence_level { get; set; }        
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
    }
}
