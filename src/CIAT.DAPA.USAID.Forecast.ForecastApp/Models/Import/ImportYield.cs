using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    public class ImportYield
    {        
        public string weather_station { get; set; }        
        public string soil { get; set; }        
        public string cultivar { get; set; }        
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string measure { get; set; }
        public double median { get; set; }
        public double avg { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double quar_1 { get; set; }
        public double quar_2 { get; set; }
        public double quar_3 { get; set; }
        public double conf_lower { get; set; }
        public double conf_upper { get; set; }
        public double sd { get; set; }
        public double perc_5 { get; set; }
        public double perc_95 { get; set; }
        public double coef_var { get; set; }
    }
}
