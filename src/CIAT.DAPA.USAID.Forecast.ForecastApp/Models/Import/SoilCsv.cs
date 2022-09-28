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

        public static Soil FromCsv(string csv_line, string tittles)
        {
            string[] values = csv_line.Split(',');
            string[] tittles_values = tittles.Split(',');
            Soil soil_csv = new Soil();
            int position = 0;
            foreach (string tittle in tittles_values)
            {

                if (tittle.Contains("crop") && soil_csv.GetType().GetProperty(tittle) != null)
                {
                    soil_csv.GetType().GetProperty(tittle).SetValue(soil_csv, ForecastDB.parseId(values[position]), null);
                }
                else if(soil_csv.GetType().GetProperty(tittle) != null)
                {
                    soil_csv.GetType().GetProperty(tittle).SetValue(soil_csv, values[position], null);
                }
                position += 1;

            }
            soil_csv.order = 0;

            return soil_csv;
        }


    }
    
}
