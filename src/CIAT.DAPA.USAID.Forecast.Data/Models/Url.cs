using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the url of the system
    /// </summary>
    public class Url
    {
        /// <summary>
        /// ID's of register of the url
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Id of the country vinculed to the url
        /// </summary>
        [Display(Name = "Country"), Required(ErrorMessage = "Country is required")]
        [BsonRequired]
        public ObjectId country { get; set; }
        /// <summary>
        /// Type of url
        /// </summary>
        [Display(Name = "Type"), Required(ErrorMessage = "Type is required")]
        [BsonRequired]
        public UrlTypes type { get; set; }
        /// <summary>
        /// Type of response vinculated to the recommendation
        /// </summary>
        [Display(Name = "Urls"), Required(ErrorMessage = "At least one url is required")]
        [BsonRequired]
        public IEnumerable<UrlData> urls { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
    }
}
