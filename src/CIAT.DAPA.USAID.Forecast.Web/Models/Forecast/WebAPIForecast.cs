using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
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
        private string Patho { get; set; }
        /// <summary>
        /// Get or set the path to get geographic information 
        /// </summary>
        private static readonly string Format = "json";
        /// <summary>
        /// Get or set the path to get geographic information 
        /// </summary>
        private static readonly string Geographic = "Geographic/";
        /// <summary>
        /// Get or set the path to get geographic information 
        /// </summary>
        private static readonly string GeographicCrop = "Geographic/Crop/";
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
        /// Get or set the path to get historical climate information
        /// </summary>
        private static readonly string ForecastYield = "Forecast/Yield/";
        /// <summary>
        /// Get or set the path to get geographic agronomic
        /// </summary>
        private static readonly string Agronomic = "Agronomic/";
        /// <summary>
        /// Get or set the path to get historical climate information
        /// </summary>
        private static readonly string Exceedance = "Forecast/YieldExceedance/";
        /// <summary>
        /// Get or set the path to get country information 
        /// </summary>
        private static readonly string Country = "Country/";
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="root"></param>
        public WebAPIForecast(string root, string patho = null)
        {
            Root = root;
            Patho = patho;
        }
                
        /// <summary>
        /// Method that makes a request to web page to get information from its
        /// </summary>
        /// <param name="path">Url to request data</param>
        /// <returns>String with the answer</returns>
        private async Task<string> RequestDataAsync(string path)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                WebRequest request = WebRequest.Create(path);
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                //ServicePointManager.DefaultConnectionLimit = 9999;                
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
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Patho + DateTime.Now.ToString("yyyyMMdd"), "1... "+ ex.Message.ToString() + "\n" + ex.InnerException.ToString() + "\n" + ex.StackTrace.ToString() + "\n");
                throw ex;
            }
        }        

        /// <summary>
        /// Method that list all municipalities from the forecast web api 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<States>> GetGeographicAsync()
        {
            string json = await RequestDataAsync(Root + Geographic + Format);
            var answer = JsonConvert.DeserializeObject<IEnumerable<States>>(json);
            return answer;
        }

        /// <summary>
        /// Method that list all municipalities from the forecast web api 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<StatesCrop>> GetGeographicCropAsync()
        {
            string json = await RequestDataAsync(Root + GeographicCrop + Format);
            var answer = JsonConvert.DeserializeObject<IEnumerable<StatesCrop>>(json);
            return answer;
        }

        /// <summary>
        /// Method that list all municipalities from the forecast web api 
        /// </summary>
        /// <returns></returns>
        public async Task<ForecastWeather> GetForecastWeatherAsync(string ws)
        {
            string json = await RequestDataAsync(Root + ForecastWeather + ws + "/true/" + Format);
            var answer = JsonConvert.DeserializeObject<ForecastWeather>(json);
            return answer;
        }

        /// <summary>
        /// Method that get all historical climateof weather station
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<HistoricalClimate>> GetHistoricalClimateAsync(string ws)
        {
            string json = await RequestDataAsync(Root + HistoricalClimate + ws + "/" + Format);
            var answer = JsonConvert.DeserializeObject<IEnumerable<HistoricalClimate>>(json);
            return answer;
        }

        /// <summary>
        /// Method that list all municipalities from the forecast web api 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<HistoricalClimatology>> GetHistoricalClimatologyAsync(string ws)
        {
            string json = await RequestDataAsync(Root + HistoricalClimatology + ws + "/" + Format);
            var answer = JsonConvert.DeserializeObject<IEnumerable<HistoricalClimatology>>(json);
            return answer;
        }

        /// <summary>
        /// Method that gets the output of yield forecast for a weather station
        /// </summary>
        /// <returns></returns>
        public async Task<ForecastYield> GetForecastYieldAsync(string ws)
        {
            string json = await RequestDataAsync(Root + ForecastYield + ws + "/" + Format);
            var answer = JsonConvert.DeserializeObject<ForecastYield>(json);
            return answer;
        }

        /// <summary>
        /// Method that gets the agronomic configuration
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Agronomic>> GetAgronomicAsync()
        {
            string json = await RequestDataAsync(Root + Agronomic + "true/" + Format);
            var answer = JsonConvert.DeserializeObject<IEnumerable<Agronomic>>(json);
            return answer;
        }

        /// <summary>
        /// Method that gets the output of yield forecast for a weather station
        /// </summary>
        /// <returns></returns>
        public async Task<ForecastYield> GetForecastYieldExceedanceAsync(string ws)
        {
            string json = await RequestDataAsync(Root + Exceedance + ws + "/" + Format);
            var answer = JsonConvert.DeserializeObject<ForecastYield>(json);
            return answer;
        }
        /// <summary>
        /// Method that list all countries from the forecast web api 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Country>> GetCountryAsync()
        {
            string json = await RequestDataAsync(Root + Country);
            var answer = JsonConvert.DeserializeObject<IEnumerable<Country>>(json);
            return answer;
        }
    }
}
