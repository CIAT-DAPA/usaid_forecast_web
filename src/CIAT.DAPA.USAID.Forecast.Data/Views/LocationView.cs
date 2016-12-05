using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Views
{
    public class LocationView
    {
        public string st_id { get; set; }
        public string st_name { get; set; }
        public string st_country { get; set; }
        public string mn_id { get; set; }
        public string mn_name { get; set; }
        public string ws_id { get; set; }
        public string ws_name { get; set; }
        public string ws_origin { get; set; }
        public string ws_ext { get; set; }
        public double ws_lat { get; set; }
        public double ws_lon { get; set; }
    }
}
