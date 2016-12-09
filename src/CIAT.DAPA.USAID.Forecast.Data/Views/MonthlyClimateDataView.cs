using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Views
{
    public class MonthlyClimateDataView
    {
        public int month { get; set; }
        public IEnumerable<ClimaticData> values { get; set; }
    }
}
