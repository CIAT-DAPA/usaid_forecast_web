using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Cultivar
    {
        /// <summary>
        /// Method that return a string representation of the cultivar
        /// </summary>
        /// <returns>String with the content of the cultivar</returns>
        public override string ToString()
        {
            return id.ToString() + "|" + name + "|" + crop.ToString() + "|" + order.ToString() + "|" + rainfed.ToString();
        }
    }
}
