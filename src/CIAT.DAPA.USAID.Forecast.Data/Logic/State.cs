using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class State
    {
        /// <summary>
        /// Method that return a string representation of the state 
        /// </summary>
        /// <returns>String with the content of the state</returns>
        public override string ToString()
        {
            return id.ToString() + "|" + name + "|" + country.ToString();
        }
    }
}
