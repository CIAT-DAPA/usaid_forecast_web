using System;
using System.Collections.Generic;
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
        /// Method constructor
        /// </summary>
        /// <param name="line"></param>
        public IndicatorModel(string line)
        {
            string[] fields = line.Split(",");
            Crop = fields[0];
            CropID = fields[1];
            Group = fields[2];
            GroupID = fields[3];
            IndicatorName = fields[4];
            IndicatorNameID = fields[5];
            Description = fields[6];
        }
    }
}
