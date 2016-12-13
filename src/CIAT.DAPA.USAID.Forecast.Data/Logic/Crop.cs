using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Crop
    {
        /// <summary>
        /// Method that return a string representation of the crop 
        /// </summary>
        /// <returns>String with the content of the crop</returns>
        public override string ToString()
        {
            return id.ToString() + "|" + name ;
        }
    }
}
