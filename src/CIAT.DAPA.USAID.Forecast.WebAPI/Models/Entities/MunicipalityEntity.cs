using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class MunicipalityEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<WeatherStationEntity> weather_stations { get; set; }
    }
}
