﻿using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories
{
    public class RepositoryWeatherStations
    {
        private WebAPIForecast Client { get; set; }

        public RepositoryWeatherStations(string root)
        {
            Client = new WebAPIForecast(root);
        }

        /// <summary>
        /// Method that returns the first weather station provide for the API
        /// </summary>
        /// <returns></returns>
        public async Task<DefaultWeatherStation> DefaultWeatherStationAsync()
        {
            var states = await Client.GetGeographicAsync();
            States s = states.FirstOrDefault();
            DefaultWeatherStation answer = new DefaultWeatherStation()
            {
                State = s.Name,
                Municipality = s.Municipalities.First().Name,
                Station = s.Municipalities.First().Weather_Stations.First().Name
            };
            return answer;
        }

        /// <summary>
        /// Method that search a weather station by its location
        /// </summary>
        /// <param name="state">Name of state</param>
        /// <param name="municipality">Name of municipality</param>
        /// <param name="ws">Name of the weather station</param>
        /// <returns></returns>
        public async Task<WeatherStation> SearchAsync(string state, string municipality, string ws)
        {
            var states = await Client.GetGeographicAsync();
            WeatherStation answer = states.Where(p => p.Name.Equals(state))
                                        .SelectMany(p => p.Municipalities)
                                        .Where(p => p.Name.Equals(municipality))
                                        .SelectMany(p => p.Weather_Stations)
                                        .SingleOrDefault(p => p.Name.Equals(ws));
            return answer;
        }
    }
}
