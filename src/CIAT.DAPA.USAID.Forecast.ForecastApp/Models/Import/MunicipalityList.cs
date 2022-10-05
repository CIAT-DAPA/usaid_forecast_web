using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    internal class MunicipalityList
    {
        public ObjectId municipality_id { get; set; }
        public ObjectId state_id { get; set; }
        public List<YieldRange> ranges { get; set; }
        public string municipality_name { get; set; }

        /// <summary>
        /// It takes a list of WeatherCsv and transforms them into a list of municipalities (MunicipalityList) with all the ranges of the municipality and its different crops
        /// </summary>
        /// <param name="weather_list"> WeatherCsv list with the ranges to add to the municipalities </param>

        public static List<MunicipalityList> CreateList(List<WeatherCsv> weather_list)
        {
            List<MunicipalityList> municipality_list = new List<MunicipalityList>();
            foreach (WeatherCsv weather in weather_list)
            {
                int position = 0;
                bool match = false;
                foreach (MunicipalityList param_to_check in municipality_list)
                {
                    if (param_to_check.municipality_id == weather.municipality_id)
                    {
                        match = true;
                        break;
                    }
                    position += 1;
                }
                if (match)
                {
                    List<YieldRange> yield_ranges = weather.ranges;
                    yield_ranges.AddRange(municipality_list[position].ranges);
                    municipality_list[position].ranges = yield_ranges;
                }
                else
                {
                    MunicipalityList new_municipality = new MunicipalityList()
                    {
                        municipality_id = weather.municipality_id,
                        state_id = weather.state_id,
                        ranges = weather.ranges,
                        municipality_name = weather.municipality_name
                    };
                    municipality_list.Add(new_municipality);
                }

            }
            return municipality_list;
        }

    }
}
