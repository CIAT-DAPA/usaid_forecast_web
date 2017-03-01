using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    public class ImportScenario
    {
        public string ws { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string measure { get; set; }
        public string scenario { get; set; }
        public double value { get; set; }

    }
}
