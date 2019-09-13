using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class ForecastScenarioMonthly
    {
        [DataMember(Name = "month")]
        public int Month { get; set; }
        [DataMember(Name = "data")]
        public IEnumerable<ForecastScenarioMonthlyData> Data { get; set; }
    }
}
