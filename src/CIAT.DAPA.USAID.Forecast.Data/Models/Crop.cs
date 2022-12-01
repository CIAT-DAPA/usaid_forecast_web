using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about crops
    /// </summary>
    public partial class Crop
    {
        /// <summary>
        /// ID's crop
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Crops' name
        /// </summary>
        //[Display(Name = "Crop name"), Required(ErrorMessage = "The crop's name is required")]
        [Display(Name = "Nombre del cultivo"), Required(ErrorMessage = "Nombre del cultivo es obligatorio")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
        /// <summary>
        /// List of Crop Config
        /// </summary>
        public IEnumerable<CropConfig> crop_config { get; set; }
    }
}
