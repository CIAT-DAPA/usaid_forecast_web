using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools
{
    public class PermissionList
    {
        public List<Country> countries { get; set; }
        public List<State> states { get; set; }
        public List<Municipality> municipalities { get; set; }
        public List<WeatherStation> weather_stations { get; set; }
        public List<Setup> setups { get; set; }
        public List<Soil> soils { get; set; }
        public List<Cultivar> cultivars { get; set; }
        public List<Recommendation> recommendations { get; set; }
        public List<Url> urls { get; set; }
    }
}
