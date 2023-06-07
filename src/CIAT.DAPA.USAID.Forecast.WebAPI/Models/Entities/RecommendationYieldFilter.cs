using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class RecommendationYieldFilter
    {
        public string ws { get; set; }
        public string cultivar { get; set; }
        public string soil { get; set; }
        public DateTime date { get; set; }
        public DateTime end_date { get; set; }
        public YieldData yield { get; set; }
    }
}
