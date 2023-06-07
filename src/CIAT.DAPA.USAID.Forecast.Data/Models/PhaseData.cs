using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the data of phenological phase
    /// </summary>
    public class PhaseData
    {
        /// <summary>
        /// Name of the phenological phase
        /// </summary>
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// start date of the phenological phase
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime start { get; set; }
        /// <summary>
        /// end date of the phenological phase
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime end { get; set; }

    }
}
