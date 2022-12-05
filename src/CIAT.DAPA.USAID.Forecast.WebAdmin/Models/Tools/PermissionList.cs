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
    }
}
