using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Indicators
{
    public sealed class IndicatorRepository
    {
        private static IndicatorRepository _instance { get; set; }
        public List<IndicatorModel> Indicators { get; set; }

        private IndicatorRepository()
        {
            Indicators = new List<IndicatorModel>();
        }

        public static IndicatorRepository GetInstance()
        {
            if (_instance == null)
                _instance = new IndicatorRepository();
            return _instance;
        }

        public async Task<bool> LoadAsync(string file)
        {
            // Read the file
            StreamReader reader = new StreamReader(file);
            string line = string.Empty;
            bool first = true;
            // Processing all lines of file
            while (!reader.EndOfStream)
            {
                line = await reader.ReadLineAsync();
                // First line is header
                if (first)
                    first = !first;
                // Others lines
                else if (!string.IsNullOrEmpty(line))
                    Indicators.Add(new IndicatorModel(line));
            }
            return true;
        }

    }
}
