using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about configurations files.
    /// </summary>
    public class ConfigurationFile
    {
        /// <summary>
        /// Configuration's name
        /// </summary>
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// Absolut path where the configuration file is located
        /// </summary>
        [BsonRequired]
        public string path { get; set; }
        /// <summary>
        /// Date of file creation in the system
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime date { get; set; }
    }
}
