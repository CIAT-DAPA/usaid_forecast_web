using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class Yield
    {
        [DataMember(Name = "cultivar")]
        public string Cultivar { get; set; }
        [DataMember(Name = "soil")]
        public string Soil { get; set; }
        [DataMember(Name = "start")]
        public DateTime Start { get; set; }
        [DataMember(Name = "end")]
        public DateTime End { get; set; }
        [DataMember(Name = "data")]
        public IEnumerable<YieldData> Data { get; set; }
    }
}
