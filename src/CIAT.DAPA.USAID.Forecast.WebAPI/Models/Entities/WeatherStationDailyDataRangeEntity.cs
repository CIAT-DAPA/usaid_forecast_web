
using System.Collections.Generic;

using CIAT.DAPA.USAID.Forecast.Data.Models;
namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    /// <summary>
    /// This entity has the daily data on the climatic variables stations.
    /// </summary>
    public class WeatherStationDailyDataRangeEntity
    {
  

    
        public string weather_station { get; set; }
        public List<DailyClimate> daily_data { get; set; } // Changed to List

        public WeatherStationDailyDataRangeEntity(string weatherStation, List<DailyClimate> dailyData)
        {
            this.weather_station = weatherStation;
            this.daily_data = dailyData ?? new List<DailyClimate>(); // Ensure it's initialized
        }

    }

}
