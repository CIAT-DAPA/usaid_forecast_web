using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories
{
    public class RepositoryForecastYield
    {
        private WebAPIForecast Client { get; set; }

        public RepositoryForecastYield(string root)
        {
            Client = new WebAPIForecast(root);
        }

        /// <summary>
        /// Method that gets the forecast information
        /// </summary>
        /// <param name="ws">Weather station ID</param>
        /// <returns></returns>
        public async Task<ForecastYield> SearchAsync(string ws)
        {
            ForecastYield answer = await Client.GetForecastYieldAsync(ws);
            return answer;
        }

        /// <summary>
        /// Method that gets the forecast Exceedance information
        /// </summary>
        /// <param name="ws">Weather station ID</param>
        /// <returns></returns>
        public async Task<ForecastYield> SearchExceedanceAsync(string ws)
        {
            ForecastYield answer = await Client.GetForecastYieldExceedanceAsync(ws);
            return answer;
        }
    }
}
