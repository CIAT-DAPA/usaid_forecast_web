using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Source
    {
        /// <summary>
        /// Method that return a string representation of the source 
        /// </summary>
        /// <returns>String with the content of the source</returns>
        public override string ToString()
        {
            return id.ToString() + "|" + name;
        }
    }
}
