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
        /// Get or set the Period name
        /// </summary>
        public string Period { get; set; }
        /// <summary>
        /// Get or set the period id
        /// </summary>
        public string PeriodID { get; set; }


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
        /// Get or set the type of variable which represents
        /// </summary>
        public char Type { get; set; }
        /// <summary>
        /// Get or set the list of categories
        /// </summary>
        public List<IndicatorCategory> Categories { get; set; }
        /// <summary>
        /// Get or set the acronym for each indicator
        /// </summary>
        public string Acronym { get; set; }


        /// <summary>
        /// Method constructor
        /// </summary>
        /// <param name="line"></param>
        public IndicatorModel(string line)
        {
            string[] fields = line.Split(",");
            Crop = fields[0].Trim();
            CropID = fields[1].Trim();
            Group = fields[2].Trim();
            GroupID = fields[3].Trim();
            IndicatorName = fields[4].Trim();
            IndicatorNameID = fields[5].Trim();
            Description = fields[6].Trim();
            Units = fields[7].Trim();
            Min = double.Parse(fields[8].Trim(), CultureInfo.InvariantCulture);
            Max = double.Parse(fields[9].Trim(), CultureInfo.InvariantCulture);
            Type = char.Parse(fields[10].Trim());
            Categories = new List<IndicatorCategory>();
            if (Type == 'c')
            {
                foreach (var c in fields[11].Trim().Split(";"))
                {
                    string[] ca = c.Split("-");
                    Categories.Add(new IndicatorCategory() { Id = ca[0].Trim(), Description = ca[1].Trim() });
                }
            }
            Acronym = fields[12].Trim();
        }
    }
}
