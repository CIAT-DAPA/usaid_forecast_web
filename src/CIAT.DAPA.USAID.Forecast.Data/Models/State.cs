using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about the states or departments.
    /// </summary>
    public partial class State
    {
        /// <summary>
        /// ID's state
        /// </summary>
        [BsonId]
        public ObjectId id { get; set; }
        /// <summary>
        /// Name of the department or state in which is located the locality
        /// </summary>
        //[Display(Name = "State name"), Required(ErrorMessage = "The state name is required")]
        [Display(Name = "Nombre del estado"), Required(ErrorMessage = "Nombre del estado es obligatorio")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// Country in which is located the state
        /// </summary>
        //[Display(Name = "Country"), Required(ErrorMessage = "The country is required")]
        [Display(Name = "País"), Required(ErrorMessage = "País es obligatorio")]
        [BsonRequired]
        public ObjectId country { get; set; }
        /// <summary>
        /// Contains the configurations of each quarter for the execution of cpt
        /// </summary>
        public IEnumerable<ConfigurationCPT> conf { get; set; }
        /// <summary>
        /// Shows the trace of the changes that occurred in the entity
        /// </summary>
        [BsonRequired]        
        public Track track { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[BsonRequired]
        public IEnumerable<ConfigurationPyCPT> conf_pycpt { get; set; }
    }
}
