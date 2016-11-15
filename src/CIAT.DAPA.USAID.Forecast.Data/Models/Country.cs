using System;


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
        public string name { get; set; }
        /// <summary>
        /// ISO 2 Code of the country
        /// </summary>
        public string iso2 { get; set; }
    }
}
