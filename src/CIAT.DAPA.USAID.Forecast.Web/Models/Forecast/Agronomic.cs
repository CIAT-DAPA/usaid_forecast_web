using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast
{
    
    public class Crop
    {
        public string cp_id { get; set; }
        public string cp_name { get; set; }
        public Soil[] soils { get; set; }
        public Cultivar[] cultivars { get; set; }
    }

    public class Soil
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Cultivar
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool rainfed { get; set; }
    }

}
