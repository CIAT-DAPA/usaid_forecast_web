using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about url
    /// </summary>
    public class UrlData
    {
        /// <summary>
        /// Name of the url
        /// </summary>
        [BsonId]
        public string name { get; set; }
        /// <summary>
        /// Url link
        /// </summary>
        [BsonRequired]
        public string value { get; set; }
        [BsonRequired]
        public ForecastType forc_type { get; set; }
        [BsonRequired]
        public CategoryUrl prob_type { get; set; }

    }
}
