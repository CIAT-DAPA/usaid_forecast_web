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
        [Display(Name = "Cultivar name"), Required(ErrorMessage = "The cultivar's name is required")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// ID's owner crop
        /// </summary>
        [Display(Name = "Crop"), Required(ErrorMessage = "The crop is required")]
        [BsonRequired]
        public ObjectId crop { get; set; }
        /// <summary>
        /// Indicates the order they should be listed
        /// </summary>
        [Display(Name = "Order to list"), Required(ErrorMessage = "The order is required")]
        [BsonRequired]
        public int order { get; set; }
        /// <summary>
        /// Sets whether or not the variety is dry. In case it is not, 
        /// it is assumed that it is irrigated
        /// </summary>
        [Display(Name = "Is rainfed"), Required(ErrorMessage = "The rainfed is required")]
        [BsonRequired]
        public bool rainfed { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
    }
}
