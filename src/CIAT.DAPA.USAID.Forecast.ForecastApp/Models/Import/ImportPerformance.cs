using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    public class ImportPerformance
    {
        public int year { get; set; }
        public int month { get; set; }
        public string ws { get; set; }
        public double pearson { get; set; }
        public double kendall { get; set; }
        public double goodness { get; set; }
    }
}
