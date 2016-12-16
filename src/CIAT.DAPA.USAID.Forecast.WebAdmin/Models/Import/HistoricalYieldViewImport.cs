using CIAT.DAPA.USAID.Forecast.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Import
{
    /// <summary>
    /// This class is a view of the importer
    /// </summary>
    public class HistoricalYieldViewImport
    {
        /// <summary>
        /// Id of the weather station
        /// </summary>
        public string weather_station { get; set; }
        /// <summary>
        /// Id of the soil
        /// </summary>
        public string soil { get; set; }
        /// <summary>
        /// Id of the cultivar
        /// </summary>
        public string cultivar { get; set; }
        /// <summary>
        /// Start date
        /// </summary>
        public DateTime start { get; set; }
        /// <summary>
        /// End date
        /// </summary>
        public DateTime end { get; set; }
        /// <summary>
        /// Measure yield
        /// </summary>
        public MeasureYield measure { get; set; }
        /// <summary>
        /// Median
        /// </summary>
        public double median { get; set; }
        /// <summary>
        /// Average
        /// </summary>
        public double avg { get; set; }
        /// <summary>
        /// Minimun value
        /// </summary>
        public double min { get; set; }
        /// <summary>
        /// Maximun value
        /// </summary>
        public double max { get; set; }
        /// <summary>
        /// First quartile
        /// </summary>
        public double quar_1 { get; set; }
        /// <summary>
        /// Second quartile
        /// </summary>
        public double quar_2 { get; set; }
        /// <summary>
        /// Third quartile
        /// </summary>
        public double quar_3 { get; set; }
        /// <summary>
        /// Limit lower confidence
        /// </summary>
        public double conf_lower { get; set; }
        /// <summary>
        /// Limit upper confidence
        /// </summary>
        public double conf_upper { get; set; }
        /// <summary>
        /// Standard desviation
        /// </summary>
        public double sd { get; set; }
        /// <summary>
        /// 5 percentile
        /// </summary>
        public double perc_5 { get; set; }
        /// <summary>
        /// 95 percentile
        /// </summary>
        public double perc_95 { get; set; }
        /// <summary>
        /// coefficient of variation
        /// </summary>
        public double coef_var { get; set; }
    }
}
