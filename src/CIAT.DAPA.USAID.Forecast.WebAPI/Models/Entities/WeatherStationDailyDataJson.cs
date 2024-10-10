
using System.Collections.Generic;

using CIAT.DAPA.USAID.Forecast.Data.Models;
namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    /// <summary>
    /// This entity has the daily data on the climatic variables stations.
    /// </summary>
    public class WeatherStationDailyDataJson
    {

        public string weather_station { get; set; }
        public string date { get; set; }
        public IEnumerable<ClimaticData> climaticData { get; set; }

    }

}
