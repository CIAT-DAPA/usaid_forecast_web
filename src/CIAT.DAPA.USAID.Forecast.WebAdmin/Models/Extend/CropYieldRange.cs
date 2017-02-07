using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Extend
{
    public class CropYieldRange : YieldRange
    {
        /// <summary>
        /// Get or set the name of the crop
        /// </summary>
        public string crop_name { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        public CropYieldRange()
        {
        }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="range">Yield Range entity</param>
        /// <param name="cp">Crops' list</param>
        public CropYieldRange(YieldRange range, List<Crop> cp)
        {
            this.crop = range.crop;
            this.label = range.label;
            this.lower = range.lower;
            this.upper = range.upper;
            this.crop_name = cp.SingleOrDefault(p => p.id == range.crop).name;
        }
    }
}
