using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the data of crop yield for different variables
    /// </summary>
    public class YieldData
    {
        /// <summary>
        /// Name of the measured variable
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public MeasureYield measure { get; set; }
        /// <summary>
        /// Median
        /// </summary>
        [BsonRequired]
        public double median { get; set; }
        /// <summary>
        /// Average
        /// </summary>
        [BsonRequired]
        public double avg { get; set; }
        /// <summary>
        /// Minimum value
        /// </summary>
        [BsonRequired]
        public double min { get; set; }
        /// <summary>
        /// Maximum value
        /// </summary>
        [BsonRequired]
        public double max { get; set; }
        /// <summary>
        /// first quartile
        /// </summary>
        [BsonRequired]
        public double quar_1 { get; set; }
        /// <summary>
        /// second quartile
        /// </summary>
        [BsonRequired]
        public double quar_2 { get; set; }
        /// <summary>
        /// third quartile
        /// </summary>
        [BsonRequired]
        public double quar_3 { get; set; }
        /// <summary>
        /// Lower limit of confidence interval
        /// </summary>
        [BsonRequired]
        public double conf_lower { get; set; }
        /// <summary>
        /// Upper limit of the confidence interval
        /// </summary>
        [BsonRequired]
        public double conf_upper { get; set; }
        /// <summary>
        /// Standard deviation
        /// </summary>
        [BsonRequired]
        public double sd { get; set; }
        /// <summary>
        /// 5th percentile
        /// </summary>
        [BsonRequired]
        public double perc_5 { get; set; }
        /// <summary>
        /// 95th percentile
        /// </summary>
        [BsonRequired]
        public double perc_95 { get; set; }
        /// <summary>
        /// Variation Conficiente
        /// </summary>
        [BsonRequired]
        public double coef_var { get; set; }

    }
}
