using System;
using System.Collections.Generic;
using System.Linq;
using CIAT.DAPA.USAID.Forecast.Data.Enums;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Export
{
    public class AreasCpt
    {
        public string predictor { get; set; }
        public string x_min { get; set; }
        public string x_max { get; set; }
        public string y_min { get; set; }
        public string y_max { get; set; }

    }
}
