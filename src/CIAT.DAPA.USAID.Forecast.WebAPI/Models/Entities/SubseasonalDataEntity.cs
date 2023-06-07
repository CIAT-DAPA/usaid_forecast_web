using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class SubseasonalDataEntity
    {
        public int year { get; set; }
        public int month { get; set; }
        public int week { get; set; }
        public IEnumerable<ProbabilityEntity> probabilities { get; set; }
    }
}
