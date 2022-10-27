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
    public partial class WeatherStation
    {
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// weather station's name
        /// </summary>
        //[Display(Name = "Weather station name"), Required(ErrorMessage = "The weather station's name is required")]
        [Display(Name = "Nombre de estación climática"), Required(ErrorMessage = "Nombre de estación climática es obligatorio")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// Id of the data source (external id)
        /// </summary>
        //[Display(Name = "External code"), Required(ErrorMessage = "The external code is required")]
        [Display(Name = "Código externo"), Required(ErrorMessage = "Código externo es obligatorio")]
        [BsonRequired]
        public string ext_id { get; set; }
        /// <summary>
        /// ID's municipality where weather station is located
        /// </summary>
        //[Display(Name = "Municipality"), Required(ErrorMessage = "The municipality is required")]
        [Display(Name = "Municipio"), Required(ErrorMessage = "Municipio es obligatorio")]
        [BsonRequired]
        public ObjectId municipality { get; set; }
        /// <summary>
        /// Name of entity owns the station
        /// </summary>
        //[Display(Name = "Source of origin"), Required(ErrorMessage = "The origin is required")]
        [Display(Name = "Fuente de origen"), Required(ErrorMessage = "Fuente de origen es obligatorio")]
        [BsonRequired]
        public string origin { get; set; }
        /// <summary>
        /// Decimal latitude of the location of the station
        /// </summary>
        //[Display(Name = "Decimal latitude"), Required(ErrorMessage = "The latitude is required")]
        [Display(Name = "Latitud"), Required(ErrorMessage = "Latitud es obligatorio")]
        [BsonRequired]
        public double latitude { get; set; }
        /// <summary>
        /// Decimal longitude of the location of the station
        /// </summary>
        //[Display(Name = "Decimal longitude"), Required(ErrorMessage = "The longitude is required")]
        [Display(Name = "Longitud"), Required(ErrorMessage = "Longitud es obligatorio")]
        [BsonRequired]
        public double longitude { get; set; }
        /// <summary>
        /// Elevation of the weather station
        /// </summary>
        //[Display(Name = "Elevation")]
        [Display(Name = "Elevación")]
        public double elevation { get; set; }
        /// <summary>
        /// Array of configuration files
        /// </summary>
        [BsonRequired]
        public IEnumerable<ConfigurationFile> conf_files { get; set; }
        /// <summary>
        /// Array of yield ranges of the crops for the station
        /// </summary>        
        [BsonRequired]
        public IEnumerable<YieldRange> ranges { get; set; }
        /// <summary>
        /// Indicates if the station is visible to queries via the web service
        /// </summary>
        //[Display(Name = "Visible"), Required(ErrorMessage = "The visible is required")]
        [Display(Name = "Visible"), Required(ErrorMessage = "Es obligatorio establecer si es visible o no")]
        [BsonRequired]
        public bool visible { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
    }
}
