using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories
{
    public class RepositoryHistoricalClimate
    {
        private WebAPIForecast Client { get; set; }

        public RepositoryHistoricalClimate(string root)
        {
            Client = new WebAPIForecast(root);
        }

        /// <summary>
        /// Method that gets the forecast information
        /// </summary>
        /// <param name="ws">Weather station ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<HistoricalClimate>> SearchAsync(string ws)
        {
            IEnumerable<HistoricalClimate> answer = await Client.GetHistoricalClimateAsync(ws);
            return answer;
        }
    }
}
