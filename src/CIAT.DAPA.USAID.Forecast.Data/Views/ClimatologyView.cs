using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Views
{
    public class ClimatologyView
    {
        public string ws_id { get; set; }
        public IEnumerable<MonthlyClimateDataView> monthly_data { get; set; }

    }
}
