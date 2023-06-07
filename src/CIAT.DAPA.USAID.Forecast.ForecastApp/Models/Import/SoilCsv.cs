using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    
    internal class SoilCsv
    {

        /// <summary>
        /// It dynamically takes the values of each row of the csv that are passed by parameters and transforms them into a data of type Soil
        /// </summary>
        /// <param name="csv_line"> Is a string that contains a row of the csv where each column is separated by "," 
        /// the order of the columns does not matter, only the ranges must be the last column. </param>
        /// <param name="titles"> string separared by "," where it contains the titles of the csv that is the first row in order to dynamically take the data
        /// The titles must be named the same as the attributes of the WeatherCsv class </param>

        public static Soil FromCsv(string csv_line, string titles)
        {
            string[] values = csv_line.Split(',');
            string[] titles_values = titles.Split(',');
            Soil soil_csv = new Soil();
            int position = 0;
            foreach (string title in titles_values)
            {

                if ((title.Contains("crop") || title.Contains("country")) && soil_csv.GetType().GetProperty(title) != null)
                {
                    soil_csv.GetType().GetProperty(title).SetValue(soil_csv, ForecastDB.parseId(values[position]), null);
                }
                else if (title.Contains("order") && soil_csv.GetType().GetProperty(title) != null)
                {
                    soil_csv.GetType().GetProperty(title).SetValue(soil_csv, int.Parse(values[position]), null);
                }
                else if(soil_csv.GetType().GetProperty(title) != null)
                {
                    soil_csv.GetType().GetProperty(title).SetValue(soil_csv, values[position], null);
                }
                position += 1;

            }

            return soil_csv;
        }


    }
    
}
