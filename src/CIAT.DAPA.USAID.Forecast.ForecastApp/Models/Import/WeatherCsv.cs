using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Import
{
    internal class WeatherCsv
    {
        public string country_name { get; set; }
        public string country_iso2 { get; set; }
        public ObjectId state_id { get; set; }
        public string state_name { get; set; }
        public ObjectId municipality_id { get; set; }
        public string municipality_name { get; set; }
        public ObjectId crop_id { get; set; }
        public List<YieldRange> ranges { get; set; }

        public static WeatherCsv FromCsv(string csv_line, string tittles)
        {
            string[] values = csv_line.Split(',');
            string[] tittles_values = tittles.Split(',');
            WeatherCsv weather = setWeatherProperties(values, tittles_values);

            return weather;
        }

        private static WeatherCsv setWeatherProperties(string[] values, string[] tittles_values)
        {
            WeatherCsv weather_csv = new WeatherCsv();
            int position = 0;
            foreach (string tittle in tittles_values)
            {
                if (position == tittles_values.Length - 1)
                {
                    List<string> string_list = new List<string>();

                    for (int i = 0; i < values.Length - position; i++)
                    {
                        string_list.Add(values[position + i].Replace("\"", ""));
                    }

                    ObjectId crop_id = (ObjectId)weather_csv.GetType().GetProperty("crop_id").GetValue(weather_csv, null);
                    List<YieldRange> ranges_list = arrayToListOfRanges(string_list.ToArray(), crop_id);
                    weather_csv.GetType().GetProperty(tittle).SetValue(weather_csv, ranges_list, null);
                }
                else
                {
                    if (tittle.Contains("_id") && weather_csv.GetType().GetProperty(tittle) != null)
                    {
                        weather_csv.GetType().GetProperty(tittle).SetValue(weather_csv, ForecastDB.parseId(values[position]), null);
                    }
                    else if (weather_csv.GetType().GetProperty(tittle) != null)
                    {
                        weather_csv.GetType().GetProperty(tittle).SetValue(weather_csv, values[position], null);
                    }
                }


                position += 1;
            }
            return weather_csv;
        }

        private static List<YieldRange> arrayToListOfRanges(string[] array_ranges, ObjectId crop_id)
        {
            List<YieldRange> list_ranges = new List<YieldRange>();
            foreach (string range in array_ranges)
            {
                string label = range.Split(':')[0];
                string[] range_values = range.Split(':')[1].Split('-');
                double lower = double.Parse(range_values[0]);
                double upper = double.Parse(range_values[1]);

                YieldRange yield_range = new YieldRange()
                {
                    crop = crop_id,
                    label = label,
                    lower = lower,
                    upper = upper,
                };
                list_ranges.Add(yield_range);

            }
            return list_ranges;
        }

    }
}