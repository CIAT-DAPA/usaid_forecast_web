using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about different soil types
    /// </summary>
    public partial class Soil
    {
        /// <summary>
        /// ID's soil
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Soil's name
        /// </summary>
        //[Display(Name = "Soil name"), Required(ErrorMessage = "The soil's name is required")]
        [Display(Name = "Nombre de suelo"), Required(ErrorMessage = "Nombre de suelo es obligatorio")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// ID's owner crop
        /// </summary>
        //[Display(Name = "Crop"), Required(ErrorMessage = "The crop is required")]
        [Display(Name = "Cultivo"), Required(ErrorMessage = "Cultivo es obligatorio")]
        [BsonRequired]
        public ObjectId crop { get; set; }
        /// <summary>
        /// Indicates the order they should be listed
        /// </summary>
        //[Display(Name = "Order to list"), Required(ErrorMessage = "The order is required")]
        [Display(Name = "Orden"), Required(ErrorMessage = "El orden es obligatorio")]
        [BsonRequired]
        public int order { get; set; }
        /// <summary>
        /// List of soil thresholds
        /// </summary>    
        public IEnumerable<Threshold> threshold { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
        /// <summary>
        /// Country in which is located this soil
        /// </summary>
        [Display(Name = "País"), Required(ErrorMessage = "Es obligatorio indicar el país")]
        [BsonRequired]
        public ObjectId country { get; set; }
    }
}
