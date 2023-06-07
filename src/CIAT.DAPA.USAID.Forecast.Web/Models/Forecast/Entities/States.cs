using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class States
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "country")]
        public CountryEntity Country { get; set; }
        [DataMember(Name = "municipalities")]
        public IEnumerable<Municipality> Municipalities { get; set; }

    }
}
