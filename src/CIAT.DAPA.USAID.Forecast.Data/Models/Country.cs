using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity has the information about country. It is embedded in the locality entity
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Country's name
        /// </summary>
        //[Display(Name = "Country name"), Required(ErrorMessage = "The name of country is required")]
        [Display(Name = "Nombre del país"), Required(ErrorMessage = "Nombre del país es obligatorio")]
        [BsonRequired]
        public string name { get; set; }
        /// <summary>
        /// ISO 2 Code of the country
        /// </summary>
        //[Display(Name = "ISO 2 Code"), Required(ErrorMessage = "The ISO 2 Code is required")]
        [Display(Name = "Código ISO 2 del país"), Required(ErrorMessage = "Código ISO 2 del país es obligatorio")]
        [BsonRequired]
        public string iso2 { get; set; }
    }
}
