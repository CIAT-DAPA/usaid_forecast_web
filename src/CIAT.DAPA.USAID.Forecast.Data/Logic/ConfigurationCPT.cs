using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class ConfigurationCPT
    {
        /// <summary>
        /// Method that return a string representation of the configuration cpt 
        /// </summary>
        /// <returns>String with the content of the configuration cpt</returns>
        public override string ToString()
        {
            return cca_mode.ToString() + "|" + gamma + "|" + regions.Count().ToString() + "|" 
                  + "|" +  x_mode.ToString() + "|" + y_mode.ToString() + "|" + trimester.ToString();
        }
    }
}
