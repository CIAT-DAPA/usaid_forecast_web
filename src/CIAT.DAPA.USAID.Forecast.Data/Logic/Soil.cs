using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Soil
    {
        /// <summary>
        /// Method that return a string representation of the soil
        /// </summary>
        /// <returns>String with the content of the soil</returns>
        public override string ToString()
        {
            return id.ToString() + "|" + name + "|" + crop.ToString() + "|" + order.ToString();
        }
    }
}
