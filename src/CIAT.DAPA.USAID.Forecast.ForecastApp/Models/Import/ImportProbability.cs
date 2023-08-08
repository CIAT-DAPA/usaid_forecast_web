using System;
using System.Collections.Generic;
using System.Linq;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    public class ImportProbability
    {
        public int year { get; set; }
        public int month { get; set; }
        public string ws { get; set; }
        public double below { get; set; }
        public double normal { get; set; }
        public double above { get; set; }
        public Quarter season { get; set; }
        public string predictand { get; set; }
    }
}
