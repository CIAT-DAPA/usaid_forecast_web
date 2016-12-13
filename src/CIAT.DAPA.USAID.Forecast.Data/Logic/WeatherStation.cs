using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class WeatherStation
    {
        /// <summary>
        /// Method that return a string representation of the weather station 
        /// </summary>
        /// <returns>String with the content of the weather station</returns>
        public override string ToString()
        {
            return id.ToString() + "|" + name + "|" + municipality.ToString() + "|" + 
                    origin + "|" + ext_id + "|" + visible.ToString() + "|" + latitude.ToString() + "|" + longitude.ToString() ;
        }
    }
}
