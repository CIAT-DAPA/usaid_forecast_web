using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information of data sources for historical data
    /// </summary>
    public partial class Source
    {
        /// <summary>
        /// ID's source
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name of the source
        /// </summary>
        //[Display(Name = "Source name"), Required(ErrorMessage = "The name source is required")]
        [Display(Name = "Nombre de la fuente"), Required(ErrorMessage = "Nombre de la fuente es obligatorio")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
    }
}
