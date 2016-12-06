using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Views
{
    public class AgronomicView
    {
        public string cp_id { get; set; }
        public string cp_name { get; set; }       
        public IEnumerable<SoilView> soils { get; set; }
        public IEnumerable<CultivarView> cultivars { get; set; }
    }
}
