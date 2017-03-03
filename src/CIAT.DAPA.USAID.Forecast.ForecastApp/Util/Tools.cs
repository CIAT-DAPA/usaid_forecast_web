using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Util
{
    public class Tools
    {
        /// <summary>
        /// Method to transform the crop name into a folder names
        /// </summary>
        /// <param name="crop">Name of crop</param>
        /// <returns>String with a machine name of the crops</returns>
        public static string folderCropName(string crop)
        {
            return crop.ToLower().Replace("í", "i");
        }
    }
}
