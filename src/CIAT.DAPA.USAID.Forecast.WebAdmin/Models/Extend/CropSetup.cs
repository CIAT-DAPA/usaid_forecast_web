using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Extend
{
    public class CropSetup : Setup
    {
        /// <summary>
        /// Get or set the name of the weather station
        /// </summary>
        public string weather_station_name { get; set; }
        /// <summary>
        /// Get or ser the name of the cultivar
        /// </summary>
        public string cultivar_name { get; set; }
        /// <summary>
        /// Get or set the name of the soil
        /// </summary>
        public string soil_name { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        public CropSetup()
        {
        }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="setup">Setup entity</param>
        /// <param name="ws">Weather station's list</param>
        /// <param name="cu">Cultivar's list</param>
        /// <param name="so">Soil's list</param>
        public CropSetup(Setup setup, List<WeatherStation> ws, List<Cultivar> cu, List<Soil> so)
        {
            this.conf_files = setup.conf_files;
            this.cultivar = setup.cultivar;
            this.days = setup.days;
            this.soil = setup.soil;
            this.track = setup.track;
            this.weather_station = setup.weather_station;
            var cultivar = cu.SingleOrDefault(p => p.id == setup.cultivar);
            this.cultivar_name = cultivar == null ? string.Empty : cultivar.name;
            var soil = so.SingleOrDefault(p => p.id == setup.soil);
            this.soil_name = soil == null ? string.Empty : soil.name;
            var weather = ws.SingleOrDefault(p => p.id == setup.weather_station);
            this.weather_station_name = weather == null ? string.Empty : weather.name;
        }
    }
}
