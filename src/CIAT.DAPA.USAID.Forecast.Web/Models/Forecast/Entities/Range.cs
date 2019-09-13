using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class Range
    {
        [DataMember(Name = "crop_id")]
        public string CropId { get; set; }
        [DataMember(Name = "crop_name")]
        public string CropName { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "lower")]
        public double Lower { get; set; }
        [DataMember(Name = "upper")]
        public double Upper { get; set; }
    }
}
