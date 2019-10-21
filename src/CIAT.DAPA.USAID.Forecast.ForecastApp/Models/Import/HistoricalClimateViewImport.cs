using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    public class HistoricalClimateViewImport
    {
        /// <summary>
        /// Year of the historical data
        /// </summary>
        public int year { get; set; }
        /// <summary>
        /// Month of the historical data
        /// </summary>
        public int month { get; set; }
        /// <summary>
        /// Extern id of the weather station
        /// </summary>
        public string ext_id { get; set; }
        /// <summary>
        /// Name of the weather station
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Value of the climate data
        /// </summary>
        public double value { get; set; }
    }
}
