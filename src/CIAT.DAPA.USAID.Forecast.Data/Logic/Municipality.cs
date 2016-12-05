using CIAT.DAPA.USAID.Forecast.Data.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Municipality
    {
        /// <summary>
        /// Method that return a string representation of the municipality 
        /// </summary>
        /// <returns>String with the content of the municipality</returns>
        public override string ToString()
        {
            return id.ToString() + "|" + state.ToString() + "|" + name;
        }
    }
}
