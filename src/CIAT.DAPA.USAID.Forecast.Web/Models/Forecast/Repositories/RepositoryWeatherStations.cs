using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
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

        public RepositoryWeatherStations(string root, string id_country)
        {
            Client = new WebAPIForecast(root, id_country);
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

        /// <summary>
        /// Method that list all weather station by its location
        /// </summary>
        /// <returns></returns>
        public async Task<List<WeatherStationFull>> ListAsync()
        {
            try
            {
                var states = await Client.GetGeographicAsync();
                List<WeatherStationFull> answer = new List<WeatherStationFull>();
                foreach (var s in states)
                {
                    foreach (var m in s.Municipalities)
                    {
                        foreach (var w in m.Weather_Stations)
                            answer.Add(new WeatherStationFull()
                            {
                                Ext_Id = w.Ext_Id,
                                Id = w.Id,
                                Latitude = w.Latitude,
                                Longitude = w.Longitude,
                                Name = w.Name,
                                Station = w.Name,
                                Origin = w.Origin,
                                Ranges = w.Ranges,
                                Country = s.Country.id,
                                State = s.Name,
                                Municipality = m.Name
                            });
                    }
                }
                return answer;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Method that list all weather station by its location
        /// </summary>
        /// <returns></returns>
        public async Task<List<WeatherStationFullCrop>> ListByCropAsync()
        {
            var crops = await Client.GetGeographicCropAsync();
            List<WeatherStationFullCrop> answer = new List<WeatherStationFullCrop>();
            foreach (var c in crops)
            {
                foreach (var s in c.States)
                {
                    foreach (var m in s.Municipalities)
                    {
                        foreach (var w in m.Weather_Stations)
                            answer.Add(new WeatherStationFullCrop()
                            {
                                Ext_Id = w.Ext_Id,
                                Id = w.Id,
                                Latitude = w.Latitude,
                                Longitude = w.Longitude,
                                Name = w.Name,
                                Station = w.Name,
                                Origin = w.Origin,
                                Ranges = w.Ranges,
                                Country = s.Country.id,
                                State = s.Name,
                                Municipality = m.Name,
                                CropId = c.Id,
                                Crop = c.Name
                            });
                    }
                }
            }

            return answer;
        }
        /*public async Task<List<Country>> ListCountryAsync()
        {
            var countries = await Client.GetCountryAsync();
            var listCountries = countries.ToList();
            return listCountries;
        }*/
    }
}
