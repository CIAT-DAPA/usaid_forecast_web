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
        public double canonica { get; set; }
        public double afc2 { get; set; }
        public double groc { get; set; }
        public double ignorance { get; set; }
        public double rpss { get; set; }
        public double spearman { get; set; }

    }
}
