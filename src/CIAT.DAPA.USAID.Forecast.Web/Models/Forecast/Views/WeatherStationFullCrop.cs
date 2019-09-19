using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Views
{
    public class WeatherStationFullCrop: WeatherStationFull
    {        
        public string CropId { get; set; }
        public string Crop { get; set; }
    }
}
