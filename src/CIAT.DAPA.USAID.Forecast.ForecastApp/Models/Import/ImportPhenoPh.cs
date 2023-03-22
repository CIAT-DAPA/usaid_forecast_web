using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    public class ImportPhenoPhase
    {        
        public string weather_station { get; set; }        
        public string soil { get; set; }        
        public string cultivar { get; set; }
        public string  forecast { get; set; }
        public string name { get; set; }
        public DateTime start_phase_date { get; set; }
        public DateTime end_phase_date { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        

    }
}
