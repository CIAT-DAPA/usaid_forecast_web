using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class YieldWeatherStation
    {
        [DataMember(Name = "weather_station")]
        public string Weather_Station { get; set; }
        [DataMember(Name = "yield")]
        public IEnumerable<Yield> Yield { get; set; }
    }
}
