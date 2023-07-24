using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Export
{
    public class UrlsJsonFormat
    {
        public bool status { get; set; }
        public string msg { get; set; }
        public List<object> urls { get; set; }

    }
}
