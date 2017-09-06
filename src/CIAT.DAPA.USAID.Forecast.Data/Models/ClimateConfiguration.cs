using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the cpt configurations by state taken into account at the time of the climatic forecast
    /// </summary>
    public class ClimateConfiguration
    {
        /// <summary>
        /// ID's state
        /// </summary>
        [BsonId]
        public ObjectId state { get; set; }
        /// <summary>
        /// Contains the configurations of each quarter for the execution of cpt
        /// </summary>
        [BsonRequired]
        public IEnumerable<ConfigurationCPT> conf { get; set; }
    }
}
