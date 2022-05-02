using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RangeParameter
    {
        /// <summary>
        /// Min value
        /// </summary>
        [BsonRequired]
        public int min { get; set; }

        /// <summary>
        /// Max value
        /// </summary>
        [BsonRequired]
        public int max { get; set; }

    }
}
