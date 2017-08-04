using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the geographic coordinates
    /// </summary>
    public class Coords
    {
        /// <summary>
        /// Decimal latitude of the location
        /// </summary>
        [BsonRequired]
        public double lat { get; set; }
        /// <summary>
        /// Decimal longitude of the location
        /// </summary>
        [BsonRequired]
        public double lon { get; set; }
    }
}
