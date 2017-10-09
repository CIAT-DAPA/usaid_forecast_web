using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the yield of a cultivar, a soil for a weather station
    /// </summary>
    public class YieldCrop
    {        
        /// <summary>
        /// Start date of the result of prediction
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime start { get; set; }
        /// <summary>
        /// End date of the result of prediction
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime end { get; set; }
        /// <summary>
        /// List of variables results yield forecasts
        /// </summary>
        [BsonRequired]
        public IEnumerable<YieldData> data { get; set; }
    }
}
