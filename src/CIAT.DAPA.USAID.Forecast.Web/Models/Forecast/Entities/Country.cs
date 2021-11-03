using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class Country
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "iso2")]
        public string Iso2 { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
