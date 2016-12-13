using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about the predictions of crop yield
    /// </summary>
    public class ForecastYield
    {
        /// <summary>
        /// ID's of register of the historical
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name where the historical data is obtained
        /// </summary>
        public ObjectId forecast { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// Data of the pronostic for the crop
        /// </summary>
        public IEnumerable<YieldCrop> yield { get; set; }
    }
}
