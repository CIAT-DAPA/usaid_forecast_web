using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class ForecastClimatePerformance
    {
        [DataMember(Name = "measure")]
        public string Measure { get; set; }
        [DataMember(Name = "value")]
        public double Value { get; set; }
        [DataMember(Name = "year")]
        public int Year { get; set; }
        [DataMember(Name = "month")]
        public int Month { get; set; }

}
}
