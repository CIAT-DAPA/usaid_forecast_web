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

        /// <summary>
        /// Method to transform a csv with the ranges of the climatic stations in a class to save in the db
        /// </summary>
        /// <param name="csv_line">Is a string that contains a row of the csv where each column is separated by "," 
        /// the order of the columns does not matter, only the ranges must be the last column.</param>
        /// <param name="titles">string separared by "," where it contains the titles of the csv that is the first row in order to dynamically take the data
        /// The titles must be named the same as the attributes of the WeatherCsv class</param>

        public static WeatherCsv FromCsv(string csv_line, string titles)
        {
            string[] values = csv_line.Split(',');
            string[] tittles_values = titles.Split(',');
            WeatherCsv weather = setWeatherProperties(values, tittles_values);

            return weather;
        }

        /// <summary>
        /// Dynamically creates a data of the type WeatherCsv taking the values that are passed as parameters
        /// </summary>
        /// <param name="values"> An Array of strings with the values of the csv row, each position is a column </param>
        /// <param name="titles_values">An Array of strings with the values of the csv titles, each position is a column</param>

        private static WeatherCsv setWeatherProperties(string[] values, string[] titles_values)
        {
            WeatherCsv weather_csv = new WeatherCsv();
            int position = 0;
            foreach (string title in titles_values)
            {
                if (position == titles_values.Length - 1)
                {
                    List<string> string_list = new List<string>();

                    for (int i = 0; i < values.Length - position; i++)
                    {
                        string_list.Add(values[position + i].Replace("\"", ""));
                    }

                    ObjectId crop_id = (ObjectId)weather_csv.GetType().GetProperty("crop_id").GetValue(weather_csv, null);
                    List<YieldRange> ranges_list = arrayToListOfRanges(string_list.ToArray(), crop_id);
                    weather_csv.GetType().GetProperty(title).SetValue(weather_csv, ranges_list, null);
                }
                else
                {
                    if (title.Contains("_id") && weather_csv.GetType().GetProperty(title) != null)
                    {
                        weather_csv.GetType().GetProperty(title).SetValue(weather_csv, ForecastDB.parseId(values[position]), null);
                    }
                    else if (weather_csv.GetType().GetProperty(title) != null)
                    {
                        weather_csv.GetType().GetProperty(title).SetValue(weather_csv, values[position], null);
                    }
                }


                position += 1;
            }
            return weather_csv;
        }

        /// <summary>
        /// Dynamically creates a ranges of the type YieldRange taking the values that are passed as parameters
        /// </summary>
        /// <param name="array_ranges"> An Array of strings with the values of the ranges of the weather stations</param>
        /// <param name="crop_id">ObjectId of the crop to which the range belongs</param>

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