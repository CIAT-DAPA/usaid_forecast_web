
using System.Collections.Generic;

using CIAT.DAPA.USAID.Forecast.Data.Models;
namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    /// <summary>
    /// This entity has the daily data on the climatic variables stations.
    /// </summary>
    public class WeatherStationDailyDataEntity
    {
  
        public string id { get; set; }
        public string weather_station { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public IEnumerable<DailyReading> daily_readings { get; set; }

    }

}
