using MongoDB.Bson;
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
        /// ID's weather station
        /// </summary>
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// ID's soil
        /// </summary>
        public ObjectId soil { get; set; }
        /// <summary>
        /// ID's cultivar
        /// </summary>
        public ObjectId cultivar { get; set; }
        /// <summary>
        /// Start date of the result of prediction
        /// </summary>
        public DateTime start { get; set; }
        /// <summary>
        /// End date of the result of prediction
        /// </summary>
        public DateTime end { get; set; }
        /// <summary>
        /// List of variables results yield forecasts
        /// </summary>
        public IEnumerable<YieldData> data { get; set; }
    }
}
