using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about country. It is embedded in the locality entity
    /// </summary>
    public partial class Country
    {
        /// <summary>
        /// ID's state
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Country's name
        /// </summary>
        //[Display(Name = "Country name"), Required(ErrorMessage = "The name of country is required")]
        [Display(Name = "Nombre del país"), Required(ErrorMessage = "Nombre del país es obligatorio")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// ISO 2 Code of the country
        /// </summary>
        //[Display(Name = "ISO 2 Code"), Required(ErrorMessage = "The ISO 2 Code is required")]
        [Display(Name = "Código ISO 2 del país"), Required(ErrorMessage = "Código ISO 2 del país es obligatorio")]
        [BsonRequired]
        public string iso2 { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
        /// <summary>
        /// Get or set the configuration for seasonal forecast using pypct
        /// </summary>
        //[BsonRequired]
        public IEnumerable<ConfigurationPyCPT> conf_pycpt { get; set; }
        /// <summary>
        /// Get or set the configuration for subseasonal forecast using pypct
        /// </summary>
        //[BsonRequired]
        public IEnumerable<ConfigurationPyCPT> subseasonal_pycpt { get; set; }
        /// <summary>
        /// Get or set the mode in which the country executes seasonal climate forecast
        /// </summary>
        public ForecastMode seasonal_mode { get; set; }
        /// <summary>
        /// Get or set the mode in which the country executes subseasonal climate forecast
        /// </summary>
        public ForecastMode subseasonal_mode { get; set; }
    }
}
