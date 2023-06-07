using MongoDB.Bson;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the crop config with the limit and upper of the variable
    /// </summary>
    public class CropConfig
    {
        /// <summary>
        /// Label for the range
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// Lower limit
        /// </summary>
        public double min { get; set; }
        /// <summary>
        /// Upper limit
        /// </summary>
        public double max { get; set; }
        /// <summary>
        /// Upper limit
        /// </summary>
        public string type { get; set; }
    }
}
