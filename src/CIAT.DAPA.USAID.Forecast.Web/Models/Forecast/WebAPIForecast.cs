﻿using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast
{
    /// <summary>
    /// This class has the bussines logic about the forcast web API
    /// </summary>
    public class WebAPIForecast
    {        
        /// <summary>
        /// Get or set the root path of the web api
        /// </summary>
        public string Root { get; set; }
        /// <summary>
        /// Get or set the path to get geographic information 
        /// </summary>
        private static readonly string Format = "json";
        /// <summary>
        /// Get or set the path to get geographic information 
        /// </summary>
        private static readonly string Geographic = "Geographic/";
        /// <summary>
        /// Get or set the path to get forecast weather information
        /// </summary>
        private static readonly string ForecastWeather = "Forecast/Climate/";
        /// <summary>
        /// Get or set the path to get historical climate information
        /// </summary>
        private static readonly string HistoricalClimate = "Historical/HistoricalClimatic/";
        /// <summary>
        /// Get or set the path to get historical climate information
        /// </summary>
        private static readonly string HistoricalClimatology = "Historical/Climatology/";
        /// <summary>
        /// Get or set the path to get geographic agronomic
        /// </summary>
        private static readonly string Agronomic = "";
        
        /// <summary>
        /// Get or set the path to get forecast information
        /// </summary>
        private static readonly string ForecastYield = "";

        public WebAPIForecast(string root)
        {
            Root = root;
        
        }
                
        /// <summary>
        /// Method that makes a request to web page to get information from its
        /// </summary>
        /// <param name="path">Url to request data</param>
        /// <returns>String with the answer</returns>
        private async Task<string> RequestDataAsync(string path)
        {
            WebRequest request = WebRequest.Create(path);
            request.Method = "GET";
            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string json = reader.ReadToEnd();
                return json;
            }
        }


        

        /// <summary>
        /// Method that list all municipalities from the forecast web api 
        /// </summary>
        /// <returns>List municipalities</returns>
        public async Task<IEnumerable<States>> GetGeographicAsync()
        {
            string json = await RequestDataAsync(Root + Geographic + Format);
            var answer = JsonConvert.DeserializeObject<IEnumerable<States>>(json);
            return answer;
        }

        /// <summary>
        /// Method that list all municipalities from the forecast web api 
        /// </summary>
        /// <returns>List municipalities</returns>
        public async Task<ForecastWeather> GetForecastWeatherAsync(string ws)
        {
            string json = await RequestDataAsync(Root + ForecastWeather + ws + "/true/" + Format);
            var answer = JsonConvert.DeserializeObject<ForecastWeather>(json);
            return answer;
        }

        /// <summary>
        /// Method that get all historical climateof weather station
        /// </summary>
        /// <returns>List municipalities</returns>
        public async Task<IEnumerable<HistoricalClimate>> GetHistoricalClimateAsync(string ws)
        {
            string json = await RequestDataAsync(Root + HistoricalClimate + ws + "/" + Format);
            var answer = JsonConvert.DeserializeObject<IEnumerable<HistoricalClimate>>(json);
            return answer;
        }

        /// <summary>
        /// Method that list all municipalities from the forecast web api 
        /// </summary>
        /// <returns>List municipalities</returns>
        public async Task<IEnumerable<HistoricalClimatology>> GetHistoricalClimatologyAsync(string ws)
        {
            string json = await RequestDataAsync(Root + HistoricalClimatology + ws + "/" + Format);
            var answer = JsonConvert.DeserializeObject<IEnumerable<HistoricalClimatology>>(json);
            return answer;
        }
        /*
        /// <summary>
        /// Method that get all configuration agronomic data from the forecast web api 
        /// </summary>
        /// <returns>Agronomic</returns>
        public async Task<IEnumerable<Crop>> getAgronomicAsync()
        {
            string json = await requestDataAsync(root + agronomic);
            var json_agronomic = JsonConvert.DeserializeObject<IEnumerable<Crop>>(json);            
            return json_agronomic;
        }

        /// <summary>
        /// Method that get all configuration agronomic data from the forecast web api 
        /// </summary>
        /// <returns>Forecast</returns>
        public async Task<Forecast> getForecastAsync()
        {
            string json = await requestDataAsync(root + forecast);
            var json_forecast = JsonConvert.DeserializeObject<Forecast>(json);
            return json_forecast;
        }*/
    }
}
