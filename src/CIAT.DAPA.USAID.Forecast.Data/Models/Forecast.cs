using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public class Forecast
    {
        /// <summary>
        /// ID's prediction process
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Start date of the process
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime start { get; set; }
        /// <summary>
        /// End date of the process
        /// </summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime end { get; set; }
        /// <summary>
        /// Level of confidence for the generation of the intervals
        /// </summary>
        [BsonRequired]
        public double confidence { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]
        public Track track { get; set; }
        /// <summary>
        /// Contains active cpt configurations for each state
        /// </summary>
        [BsonRequired]
        public IEnumerable<ClimateConfiguration> climate_conf { get; set; }
    }
}