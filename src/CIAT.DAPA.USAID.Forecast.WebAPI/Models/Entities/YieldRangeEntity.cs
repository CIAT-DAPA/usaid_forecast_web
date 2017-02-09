using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    /// <summary>
    /// This class has been defined how data structure
    /// to save data about of yield ranges for a weather station
    /// </summary>
    public class YieldRangeEntity
    {
        public string crop_id { get; set; }
        public string crop_name { get; set; }
        public string label { get; set; }
        public double lower { get; set; }
        public double upper { get; set; }
    }
}
