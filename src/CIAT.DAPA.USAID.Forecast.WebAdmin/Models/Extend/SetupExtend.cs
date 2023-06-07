using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIAT.DAPA.USAID.Forecast.Data.Models;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Extend
{
    public class SetupExtend : Setup
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
        /// Get or set the name of the crop
        /// </summary>
        public string crop_name { get; set; }

        public SetupExtend() { }

        public SetupExtend(Setup setup, List<Crop> crp, List<WeatherStation> ws, List<Cultivar> cu, List<Soil> so)
        {
            this.id = setup.id;
            this.conf_files = setup.conf_files;
            this.cultivar = setup.cultivar;
            this.days = setup.days;
            this.soil = setup.soil;
            this.track = setup.track;
            this.weather_station = setup.weather_station;
            this.crop = setup.crop;
            var crop = crp.SingleOrDefault(p => p.id == setup.crop);
            this.crop_name = crop == null ? string.Empty : crop.name;
            var cultivar = cu.SingleOrDefault(p => p.id == setup.cultivar);
            this.cultivar_name = cultivar == null ? string.Empty : cultivar.name;
            var soil = so.SingleOrDefault(p => p.id == setup.soil);
            this.soil_name = soil == null ? string.Empty : soil.name;
            var weather = ws.SingleOrDefault(p => p.id == setup.weather_station);
            this.weather_station_name = weather == null ? string.Empty : weather.name;
        }
    }
}
