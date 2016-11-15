using MongoDB.Bson;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the yield ranges of the crops in the weather station
    /// </summary>
    public class YieldRange
    {
        /// <summary>
        /// ID's crop
        /// </summary>        
        public ObjectId crop { get; set; }
        /// <summary>
        /// Label for the range
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// Lower limit
        /// </summary>
        public double lower { get; set; }
        /// <summary>
        /// Upper limit
        /// </summary>
        public double upper { get; set; }
    }
}
