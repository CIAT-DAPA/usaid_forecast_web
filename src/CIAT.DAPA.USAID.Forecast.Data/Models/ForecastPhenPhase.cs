using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about the predictions of phenological phases
    /// </summary>
    public class ForecastPhenPhase
    {
        /// <summary>
        /// ID's of register of the historical
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name where the historical data is obtained
        /// </summary>
        [BsonRequired]
        public ObjectId forecast { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        [BsonRequired]
        public ObjectId ws { get; set; }
        /// <summary>
        /// ID's soil
        /// </summary>
        [BsonRequired]
        public ObjectId soil { get; set; }
        /// <summary>
        /// ID's cultivar
        /// </summary>
        [BsonRequired]
        public ObjectId cultivar { get; set; }
        /// <summary>
        /// Data of the pronostic for the phenological phases
        /// </summary>
        [BsonRequired]
        public IEnumerable<PhaseCrop> phases_crop { get; set; }
    }
}
