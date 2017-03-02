using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents an indicator of behavior prediction models
    /// </summary>
    public class PerformanceMetric
    {
        /// <summary>
        /// Year forecast
        /// </summary>
        [BsonRequired]
        public int year { get; set; }
        /// <summary>
        /// Month forecast
        /// </summary>
        [BsonRequired]
        public int month { get; set; }
        /// <summary>
        /// Metric name
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public MeasurePerformance name { get; set; }
        /// <summary>
        /// Metric value
        /// </summary>
        [BsonRequired]
        public double value { get; set; }
    }
}
