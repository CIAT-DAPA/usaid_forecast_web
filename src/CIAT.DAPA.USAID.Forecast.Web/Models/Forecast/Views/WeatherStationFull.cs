using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Views
{
    public class WeatherStationFull: WeatherStation
    {
        [DataMember(Name = "state")]
        public string State { get; set; }
        [DataMember(Name = "municipality")]
        public string Municipality { get; set; }
    }
}
