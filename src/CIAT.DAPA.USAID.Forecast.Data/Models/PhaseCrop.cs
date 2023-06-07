using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the date of the simulation with each phase
    /// </summary>
    public class PhaseCrop
    {        
        /// <summary>
        /// Start date of the simulation
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime start { get; set; }
        /// <summary>
        /// End date of the simulation
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime end { get; set; }
        /// <summary>
        /// List of pehnological phases results forecasts
        /// </summary>
        [BsonRequired]
        public IEnumerable<PhaseData> data { get; set; }
    }
}
