using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents geographic regions in rectangular form
    /// </summary>
    public partial class Region
    {
        /// <summary>
        /// Coordinates of the lower left corner of the theoretical area
        /// </summary>
        [BsonRequired]
        public Coords left_lower { get; set; }
        /// <summary>
        /// Coordinates of the upper rigth corner of the theoretical area
        /// </summary>
        [BsonRequired]
        public Coords rigth_upper { get; set; }
    }
}
