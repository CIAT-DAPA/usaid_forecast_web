using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast
{
    /// <summary>
    /// This class has the bussines logic about the forcast web API
    /// </summary>
    public class WebAPIForecast
    {
        /// <summary>
        /// Get or set the object HTTP to get information from web API
        /// </summary>
        private WebRequest request { get; set; }
        /// <summary>
        /// Get or set the root path of the web api
        /// </summary>
        public string root { get; set; }
        /// <summary>
        /// Get or set the path to get geographic information 
        /// </summary>
        public string geographic { get; set; }
        /// <summary>
        /// Get or set the path to get geographic agronomic
        /// </summary>
        public string agronomic { get; set; }
        /// <summary>
        /// Get or set the path to get forecast information
        /// </summary>
        public string forecast { get; set; }
        /// <summary>
        /// Get or set the path to get historical information
        /// </summary>
        public string historical { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        public WebAPIForecast()
        {
        }

        /// <summary>
        /// Method that makes a request to web page to get information from its
        /// </summary>
        /// <param name="path">Url to request data</param>
        /// <returns>String with the answer</returns>
        private async Task<string> requestDataAsync(string path)
        {
            request = WebRequest.Create(path);
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
        public async Task<IEnumerable<Municipality>> getMunicipalitiesAsync()
        {
            string json = await requestDataAsync(root + geographic);
            var json_geo = JsonConvert.DeserializeObject<IEnumerable<State>>(json);
            List<Municipality> json_m = new List<Municipality>();
            foreach(var ms in json_geo)
                foreach (var m in ms.municipalities)
                    json_m.Add(m);
            return json_m;
        }

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
        }
    }
}
