using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents a geographic location of study. 
    /// It is the lowest form in which information can be contextualized. 
    /// This can be a township, village or municipality. 
    /// It is determined by the weather station where historical information is obtained
    /// </summary>
    public class Locality
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Locality's name. This may be municipality, township, village
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Country in which is located the locality
        /// </summary>
        public Country  country { get; set; }
        /// <summary>
        /// Name of the department or state in which is located the locality
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// Municipality's name in which is located the locality
        /// </summary>
        public string municipality { get; set; }
        /// <summary>
        /// Indicates whether is active or not the locality. True is active, it is not false
        /// </summary>
        public bool enable { get; set; }
        /// <summary>
        /// Date on which was register the locality to the database
        /// </summary>
        public DateTime date_register { get; set; }
        /// <summary>
        /// Date on which the last update of the locality was carried out in the database
        /// </summary>
        public DateTime date_updated { get; set; }
    }
}
