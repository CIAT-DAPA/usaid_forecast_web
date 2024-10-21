using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity contains daily data for a certain date.
    /// </summary>
    public class DailyClimate
    {

        /// <summary>
        /// day of the daily climatic data
        /// </summary>
        [BsonRequired]
        public DateTime date { get; set; }
        /// <summary>
        /// Climatic data
        /// </summary>
        [BsonRequired]
        public IEnumerable<ClimaticData> data { get; set; }


        public DailyClimate(DateTime date, IEnumerable<ClimaticData> data)
        {
            this.date = date;
            this.data = data;
        }

    }
}
