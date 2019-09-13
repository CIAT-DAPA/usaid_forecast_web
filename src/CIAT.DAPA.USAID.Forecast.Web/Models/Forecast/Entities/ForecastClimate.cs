using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class ForecastClimate
    {
        [DataMember(Name = "weather_station")]
        public string Weather_Station { get; set; }
        [DataMember(Name = "performance")]
        public IEnumerable<ForecastClimatePerformance> Performance { get; set; }
        [DataMember(Name = "data")]
        public IEnumerable<ForecastClimateData> Data { get; set; }
        
        
    }
}
