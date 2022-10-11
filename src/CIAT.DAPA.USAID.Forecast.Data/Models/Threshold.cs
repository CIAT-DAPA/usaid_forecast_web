using MongoDB.Bson;
using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the yield ranges of the crops in the weather station
    /// </summary>
    public class Threshold
    {      
        /// <summary>
        /// Label for the threshold
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// percentage of soil
        /// </summary>
        public double value { get; set; }

    }
}
