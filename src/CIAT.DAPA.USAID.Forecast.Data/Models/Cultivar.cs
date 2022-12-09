using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about different cultivar of the crop
    /// </summary>
    public partial class Cultivar
    {
        /// <summary>
        /// ID's cultivar
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Cultivar's name
        /// </summary>
        //[Display(Name = "Cultivar name"), Required(ErrorMessage = "The cultivar's name is required")]
        [Display(Name = "Nombre de la variedad"), Required(ErrorMessage = "Nombre de la variedad es obligatorio")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// ID's owner crop
        /// </summary>
        //[Display(Name = "Crop"), Required(ErrorMessage = "The crop is required")]
        [Display(Name = "Cultivo"), Required(ErrorMessage = "El cultivo es obligatorio")]
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
        /// Sets whether or not the variety is dry. In case it is not, 
        /// it is assumed that it is irrigated
        /// </summary>
        //[Display(Name = "Is rainfed"), Required(ErrorMessage = "The rainfed is required")]
        [Display(Name = "Es secano"), Required(ErrorMessage = "Es obligatorio indicar si es de secano o no")]
        [BsonRequired]
        public bool rainfed { get; set; }
        /// <summary>
        /// Sets whether the variety is national or imported
        /// </summary>
        //[Display(Name = "Is national"), Required(ErrorMessage = "The national is required")]
        [Display(Name = "Es nacional"), Required(ErrorMessage = "Es obligatorio indicar si es nacional o no")]
        [BsonRequired]
        public bool national { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
        /// <summary>
        /// List of Cultivar threshold
        /// </summary>
        public IEnumerable<Threshold> threshold { get; set; }
        /// <summary>
        /// Country in which is located this cultivar
        /// </summary>
        [Display(Name = "País"), Required(ErrorMessage = "Es obligatorio indicar el país")]
        [BsonRequired]
        public ObjectId country { get; set; }
    }
}
