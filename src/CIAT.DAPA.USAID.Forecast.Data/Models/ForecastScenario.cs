using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information of extreme and averages weather scenarios of the forecast
    /// </summary>
    public class ForecastScenario
    {
        /// <summary>
        /// ID's forecast scenario
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// ID's forecast
        /// </summary>
        public ObjectId forecast { get; set; }
        /// <summary>
        /// ID's weather station
        /// </summary>
        public ObjectId weather_station { get; set; }
        /// <summary>
        /// Scenario Name
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public ScenarioName name { get; set; }
        /// <summary>
        /// Year forecast
        /// </summary>
        public int year { get; set; }
        /// <summary>
        /// Monthly data of the weather station
        /// </summary>
        public IEnumerable<MonthlyDataStation> monthly_data { get; set; }

    }
}
