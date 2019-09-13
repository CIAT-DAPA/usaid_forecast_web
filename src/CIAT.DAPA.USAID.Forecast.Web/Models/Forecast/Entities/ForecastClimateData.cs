using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class ForecastClimateData
    {
        [DataMember(Name = "year")]
        public int Year { get; set; }
        [DataMember(Name = "month")]
        public int Month { get; set; }
        [DataMember(Name = "probabilities")]
        public IEnumerable<ForecastClimateProbability> Probabilities { get; set; }        
    }
}
