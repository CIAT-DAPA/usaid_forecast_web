using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about the historical of crop yield
    /// </summary>
    public class HistoricalYield
    {
        /// <summary>
        /// ID's of register of the historical
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name where the historical data is obtained
        /// </summary>
        [BsonRequired]
        //[Display(Name = "Id Source"), Required(ErrorMessage = "The source is required")]
        [Display(Name = "Fuente"), Required(ErrorMessage = "La fuente es obligatoria")]
        public ObjectId source { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonRequired]
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// Data of the pronostic for the crop
        /// </summary>
        /// /// <summary>
        /// ID's soil
        /// </summary>
        [BsonRequired]
        public ObjectId soil { get; set; }
        /// <summary>
        /// ID's cultivar
        /// </summary>
        [BsonRequired]
        public ObjectId cultivar { get; set; }
        [BsonRequired]
        public IEnumerable<YieldCrop> yield { get; set; }
        /// <summary>
        /// Date when the historical was added
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime date { get; set; }
    }
}
