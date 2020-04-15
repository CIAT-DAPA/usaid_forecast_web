using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Views
{
    public class Scenario
    {
        public string Measure { get; set; }     
        public int Year { get; set; }
        public int Month { get; set; }        
        public double Max { get; set; }
        public double Min { get; set; }
        public double Avg { get; set; }
    }
}
