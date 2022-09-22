using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about climate predictions
    /// </summary>
    public class ForecastClimate
    {
        /// <summary>
        /// ID's climate prediction
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// ID's forecast
        /// </summary>
        [BsonRequired]
        public ObjectId forecast { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonRequired]
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// List of probabilities
        /// </summary>
        [BsonRequired]
        public IEnumerable<ProbabilityClimate> data { get; set; }
        /// <summary>
        /// List of metrics that describe the behavior of the prediction model
        /// </summary>
        [BsonRequired]
        public IEnumerable<PerformanceMetric> performance { get; set; }
        /// <summary>
        /// List of probabilities of subseasonal
        /// </summary>
        public IEnumerable<ProbabilitySubseasonal> subseasonal { get; set; }
    }
}
