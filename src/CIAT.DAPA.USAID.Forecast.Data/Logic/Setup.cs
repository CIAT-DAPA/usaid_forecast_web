using System;
using System.Collections.Generic;
using System.Text;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Setup
    {
        /// <summary>
        /// Method that return a string representation of the setup 
        /// </summary>
        /// <returns>String with the content of the setup</returns>
        public override string ToString()
        {
            return id.ToString() + "";
        }
    }
}
