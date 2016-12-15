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
    /// This entity represents the probabilities of the variables of climate prediction
    /// </summary>
    public class Probability
    {
        /// <summary>
        /// Variable's name
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public MeasureClimatic measure { get; set; }
        /// <summary>
        /// Probability that is below normal
        /// </summary>
        [BsonRequired]
        public double lower { get; set; }
        /// <summary>
        /// Probability that is normal
        /// </summary>
        [BsonRequired]
        public double normal { get; set; }
        /// <summary>
        /// Normal probability that is above
        /// </summary>
        [BsonRequired]
        public double upper { get; set; }
    }
}
