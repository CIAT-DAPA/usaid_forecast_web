using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class WeatherStationEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string origin { get; set; }
        public IEnumerable<YieldRange> ranges { get; set; }
    }
}
