using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    /// <summary>
    /// This entity represents the history that an entity has had
    /// </summary>
    public class Track
    {
        /// <summary>
        /// It indicates whether the entity is active or not. True is active, it is not false
        /// </summary>
        public bool enable { get; set; }
        /// <summary>
        /// Date on which was register the entity to the database
        /// </summary>
        public DateTime register { get; set; }
        /// <summary>
        /// Date on which the last update of the entity was carried out in the database
        /// </summary>
        public DateTime updated { get; set; }
    }
}
