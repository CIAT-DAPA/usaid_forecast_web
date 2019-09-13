using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class ForecastScenarioMonthlyData
    {
        [DataMember(Name = "measure")]
        public string Measure { get; set; }
        [DataMember(Name = "value")]
        public double Value { get; set; }
    }
}
