using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class ForecastYield
    {
        [DataMember(Name = "forecast")]
        public string Forecast { get; set; }
        [DataMember(Name = "confidence")]
        public double Confidence { get; set; }
        [DataMember(Name = "yield")]
        public IEnumerable<YieldWeatherStation> Yield { get; set; }
    }
}
