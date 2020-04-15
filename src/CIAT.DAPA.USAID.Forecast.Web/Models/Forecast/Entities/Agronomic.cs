using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class Agronomic
    {
        [DataMember(Name = "cp_id")]
        public string Cp_Id { get; set; }
        [DataMember(Name = "cp_name")]
        public string Cp_Name { get; set; }
        [DataMember(Name = "soils")]
        public IEnumerable<Soil> Soils { get; set; }
        [DataMember(Name = "cultivars")]
        public IEnumerable<Cultivar> Cultivars { get; set; }
    }
}
