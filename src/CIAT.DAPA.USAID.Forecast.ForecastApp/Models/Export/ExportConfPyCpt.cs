using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIAT.DAPA.USAID.Forecast.Data.Models;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Export
{
    public class ExportConfPyCpt
    {
        public SpatialCoords spatial_predictors { get; set; }
        public SpatialCoords spatial_predictands { get; set; }
        public IEnumerable<string> models { get; set; }
        public string obs { get; set; }
        public bool station { get; set; }
        public string mos { get; set; }
        public string predictand { get; set; }
        public string predictors { get; set; }
        public IEnumerable<string> mons { get; set; }
        public IEnumerable<string> tgtii { get; set; }
        public IEnumerable<string> tgtff { get; set; }
        public IEnumerable<string> tgts { get; set; }
        public int tini { get; set; }
        public int tend { get; set; }
        public int xmodes_min { get; set; }
        public int xmodes_max { get; set; }
        public int ymodes_min { get; set; }
        public int ymodes_max { get; set; }
        public int ccamodes_min { get; set; }
        public int ccamodes_max { get; set; }
        public bool force_download { get; set; }
        public bool single_models { get; set; }
        public bool forecast_anomaly { get; set; }
        public bool forecast_spi { get; set; }
        public int confidence_level { get; set; }
        public int ind_exec { get; set; }
    }
}
