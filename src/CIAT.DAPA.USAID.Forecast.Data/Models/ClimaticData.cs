using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents climatic data
    /// </summary>
    public class ClimaticData
    {
        /// <summary>
        /// Variable's name
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]  // JSON.Net
        [BsonRequired]
        public MeasureClimatic measure { get; set; }
        /// <summary>
        /// Variable's value
        /// </summary>
        [BsonRequired]
        public double value { get; set; }
    }
}
