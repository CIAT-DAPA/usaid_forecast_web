using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the recommendation to use in the API
    /// </summary>
    public class Recommendation
    {
        /// <summary>
        /// ID's of register of the recommendation
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Id of the country vinculed to the recommendation
        /// </summary>
        [Display(Name = "Country"), Required(ErrorMessage = "Country is required")]
        [BsonRequired]
        public ObjectId country { get; set; }
        /// <summary>
        /// Type of enum vinculated to the recommendation
        /// </summary>
        [Display(Name = "Type of enums"), Required(ErrorMessage = "Type of enums is required")]
        [BsonRequired]
        public string type_enum { get; set; }
        /// <summary>
        /// Type of response vinculated to the recommendation
        /// </summary>
        [Display(Name = "Type of response"), Required(ErrorMessage = "Type of response is required")]
        [BsonRequired]
        public string type_resp { get; set; }
        /// <summary>
        /// Response of the recommendation
        /// </summary>
        [Display(Name = "Response"), Required(ErrorMessage = "Response is required")]
        [BsonRequired]
        public string resp { get; set; }
        /// <summary>
        /// language  of the recommendation
        /// </summary>
        [Display(Name = "Language"), Required(ErrorMessage = "Response is required")]
        [BsonRequired]
        public string lang { get; set; }
    }
}
