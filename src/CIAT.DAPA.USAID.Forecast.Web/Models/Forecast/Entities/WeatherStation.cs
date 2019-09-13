using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class WeatherStation
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "ext_id")]
        public string Ext_Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "origin")]
        public string Origin { get; set; }
        [DataMember(Name = "ranges")]
        public IEnumerable<Range> Ranges { get; set; }
    }
}
