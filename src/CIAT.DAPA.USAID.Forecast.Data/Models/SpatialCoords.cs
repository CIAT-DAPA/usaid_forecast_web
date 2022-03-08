using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class SpatialCoords
    {
        /// <summary>
        /// Northernmost latitude
        /// </summary>
        [BsonRequired]
        public int nla { get; set; }
        /// <summary>
        /// Southernmost latitude
        /// </summary>
        [BsonRequired]
        public int sla { get; set; }
        /// <summary>
        /// Westernmost latitude
        /// </summary>
        [BsonRequired]
        public int wlo { get; set; }
        /// <summary>
        /// Easternmost latitude
        /// </summary>
        [BsonRequired]
        public int elo { get; set; }
    }
}
