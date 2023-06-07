using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Views
{
    public class CultivarView
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool rainfed { get; set; }
        public bool national { get; set; }
        public string country_id { get; set; }
    }
}
