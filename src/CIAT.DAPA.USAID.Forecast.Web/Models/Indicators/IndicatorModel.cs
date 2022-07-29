using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Indicators
{
    public class IndicatorModel
    {
        /// <summary>
        /// Get or set the crop label
        /// </summary>
        public string Crop { get; set; }
        /// <summary>
        /// Get or set the crop id
        /// </summary>
        public string CropID { get; set; }
        /// <summary>
        /// Get or set the group name
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// Get or set the group id
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// Get or set the indicator name
        /// </summary>
        public string IndicatorName { get; set; }
        /// <summary>
        /// Get or set the indicator id
        /// </summary>
        public string IndicatorNameID { get; set; }
        /// <summary>
        /// Get or set the indicators' description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Get or set the indicators' units
        /// </summary>
        public string Units { get; set; }
        /// <summary>
        /// Get or set the min value of the indicator
        /// </summary>
        public double Min { get; set; }
        /// <summary>
        /// Get or set the max value of the indicator
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Method constructor
        /// </summary>
        /// <param name="line"></param>
        public IndicatorModel(string line)
        {
            string[] fields = line.Split(",");
            try
            {
                Crop = fields[0];
                CropID = fields[1];
                Group = fields[2];
                GroupID = fields[3];
                IndicatorName = fields[4];
                IndicatorNameID = fields[5];
                Description = fields[6];
                Units = fields[7];
                Min = double.Parse(fields[8], CultureInfo.InvariantCulture);
                Max = double.Parse(fields[9], CultureInfo.InvariantCulture);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
