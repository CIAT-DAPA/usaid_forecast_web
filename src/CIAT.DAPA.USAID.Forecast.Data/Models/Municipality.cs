using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about municipalities.
    /// </summary>
    public partial class Municipality
    {
        /// <summary>
        /// ID's municipality
        /// </summary>
        [BsonId]        
        public ObjectId id { get; set; }
        /// <summary>
        /// Name of the department or state in which is located the locality
        /// </summary>
        [Display(Name = "Municipality name"), Required(ErrorMessage = "The municipality's name is required")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// ID's state where municipality is located
        /// </summary>
        [Display(Name = "State"), Required(ErrorMessage = "The state is required")]
        [BsonRequired]
        public ObjectId state { get; set; }
        /// <summary>
        /// Indicates if the entity is visible to queries via the web service
        /// </summary>
        [Display(Name = "Visible"), Required(ErrorMessage = "The visible is required")]
        [BsonRequired]
        public bool visible { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
    }
}
