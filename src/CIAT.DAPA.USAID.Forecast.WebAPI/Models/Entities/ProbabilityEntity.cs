using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class ProbabilityEntity
    {
        public string measure { get; set; }
        public double lower { get; set; }
        public double normal { get; set; }
        public double upper { get; set; }
    }
}
