using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class YieldData
    {
        [DataMember(Name = "measure")]
        public string Measure { get; set; }
        [DataMember(Name = "median")]
        public double Median { get; set; }
        [DataMember(Name = "avg")]
        public double Avg { get; set; }
        [DataMember(Name = "min")]
        public double Min { get; set; }
        [DataMember(Name = "max")]
        public double Max { get; set; }
        [DataMember(Name = "quar_1")]
        public double Quar_1 { get; set; }
        [DataMember(Name = "quar_2")]
        public double Quar_2 { get; set; }
        [DataMember(Name = "quar_3")]
        public double Quar_3 { get; set; }
        [DataMember(Name = "conf_lower")]
        public double Conf_Lower { get; set; }
        [DataMember(Name = "conf_upper")]
        public double Conf_Upper { get; set; }
        [DataMember(Name = "sd")]
        public double Sd { get; set; }
        [DataMember(Name = "perc_5")]
        public double Perc_5 { get; set; }
        [DataMember(Name = "perc_95")]
        public double Perc_95 { get; set; }
        [DataMember(Name = "coef_var")]
        public double Coef_Var { get; set; }
    }
}
