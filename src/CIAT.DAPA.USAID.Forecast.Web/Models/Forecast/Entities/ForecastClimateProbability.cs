using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{    
    public class ForecastClimateProbability
    {
        [DataMember(Name = "measure")]
        public string Measure { get; set; }
        [DataMember(Name = "lower")]
        public double Lower { get; set; }
        [DataMember(Name = "normal")]
        public double Normal { get; set; }
        [DataMember(Name = "upper")]
        public double Upper { get; set; }
    }
}
