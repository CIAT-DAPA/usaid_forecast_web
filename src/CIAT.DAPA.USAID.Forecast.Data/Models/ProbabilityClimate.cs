﻿using MongoDB.Bson.Serialization.Attributes;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the probabilities of the variables of values of the prediction of every weather station
    /// </summary>
    public class ProbabilityClimate
    {
        /// <summary>
        /// Year forecast
        /// </summary>
        [BsonRequired]
        public int year { get; set; }
        /// <summary>
        /// Month forecast
        /// </summary>
        [BsonRequired] 
        public int month { get; set; }
        /// <summary>
        /// Type of forecast
        /// </summary>
        [BsonRequired]
        public ForecastType type { get; set; }
        /// <summary>
        /// List of variables forecast for the month
        /// </summary>
        [BsonRequired]
        public IEnumerable<Probability> probabilities { get; set; }
    }
}
