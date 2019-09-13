using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities
{
    public class HistoricalClimate
    {
        [DataMember(Name = "weather_station")]
        public string Weather_Station { get; set; }
        [DataMember(Name = "year")]
        public int Year { get; set; }
        [DataMember(Name = "monthly_data")]
        public IEnumerable<HistoricalClimateMonthly> Monthly_Data { get; set; }
    }
}
