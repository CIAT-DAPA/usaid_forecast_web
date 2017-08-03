using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Region
    {
        /// <summary>
        /// Method that return a string representation of the region 
        /// </summary>
        /// <returns>String with the content of the region</returns>
        public override string ToString()
        {
            return left_lower.lat.ToString() + "," + left_lower.lon.ToString()
                 + "|" + rigth_upper.lat.ToString() + "," + rigth_upper.lon.ToString();
        }
        
    }
}
