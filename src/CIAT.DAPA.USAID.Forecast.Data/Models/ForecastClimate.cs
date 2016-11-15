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
        public ObjectId forecast { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// Year forecast
        /// </summary>
        public int year { get; set; }
        /// <summary>
        /// Month forecast
        /// </summary>
        public int month { get; set; }
        /// <summary>
        /// List of variables forecast for the month
        /// </summary>
        public IEnumerable<Probability> probabilities { get; set; }
        /// <summary>
        /// List of metrics that describe the behavior of the prediction model
        /// </summary>
        public IEnumerable<PerformanceMetric> performance { get; set; }
    }
}
