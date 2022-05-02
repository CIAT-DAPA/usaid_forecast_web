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

        /// <summary>
        /// Method that generate a Json object
        /// </summary>
        /// <returns>JSON Object</returns>
        public object jsonConfiguration()
        {
            return new
            {
                nla = this.rigth_upper.lat.ToString(),
                sla = this.left_lower.lat.ToString(),
                wlo = this.rigth_upper.lon.ToString(),
                elo = this.left_lower.lon.ToString()
            };
        }
        
    }
}
