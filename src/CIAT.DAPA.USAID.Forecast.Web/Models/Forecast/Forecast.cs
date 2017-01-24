using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast
{
    public class Forecast
    {
        public string forecast { get; set; }
        public float confidence { get; set; }
        public Climate[] climate { get; set; }
        public ForecastYield[] yield { get; set; }
    }

    public class Climate
    {
        public string weather_station { get; set; }
        public Performance[] performance { get; set; }
        public Datum[] data { get; set; }
    }

    public class Performance
    {
        public string measure { get; set; }
        public float value { get; set; }
    }

    public class Datum
    {
        public int year { get; set; }
        public int month { get; set; }
        public Probability[] probabilities { get; set; }
    }

    public class Probability
    {
        public string measure { get; set; }
        public float lower { get; set; }
        public float normal { get; set; }
        public float upper { get; set; }
    }

    public class ForecastYield
    {
        public string weather_station { get; set; }
        public Yield[] yield { get; set; }
    }

    public class Yield
    {
        public string cultivar { get; set; }
        public string soil { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public YieldData[] data { get; set; }
    }

    public class YieldData
    {
        public string measure { get; set; }
        public float median { get; set; }
        public float avg { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float quar_1 { get; set; }
        public float quar_2 { get; set; }
        public float quar_3 { get; set; }
        public float conf_lower { get; set; }
        public float conf_upper { get; set; }
        public float sd { get; set; }
        public float perc_5 { get; set; }
        public float perc_95 { get; set; }
        public float coef_var { get; set; }
    }

}
