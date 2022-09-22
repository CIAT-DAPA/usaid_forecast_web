using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    public class ImportSubseasonal
    {
        public int year { get; set; }
        public int month { get; set; }
        public int week { get; set; }
        public string ws { get; set; }
        public double below { get; set; }
        public double normal { get; set; }
        public double above { get; set; }
    }
}
