using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the history that an entity has had
    /// </summary>
    public class Track
    {
        /// <summary>
        /// It indicates whether the entity is active or not. True is active, it is not false
        /// </summary>
        //[Display(Name = "Enable"), Required(ErrorMessage = "The enable is required")]
        [Display(Name = "Disponible"), Required(ErrorMessage = "Disponible es obligatorio")]
        [BsonRequired]
        public bool enable { get; set; }
        /// <summary>
        /// Date on which was register the entity to the database
        /// </summary>
        //[Display(Name = "Date register"), Required(ErrorMessage = "The date register is required")]
        [Display(Name = "Fecha de registro"), Required(ErrorMessage = "Fecha de registro es obligatorio")]
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime register { get; set; }
        /// <summary>
        /// Date on which the last update of the entity was carried out in the database
        /// </summary>
        //[Display(Name = "Date updated"), Required(ErrorMessage = "The date updated is required")]
        [Display(Name = "Fecha de actualización"), Required(ErrorMessage = "Fecha de actualización es obligatorio")]
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime updated { get; set; }
    }
}
