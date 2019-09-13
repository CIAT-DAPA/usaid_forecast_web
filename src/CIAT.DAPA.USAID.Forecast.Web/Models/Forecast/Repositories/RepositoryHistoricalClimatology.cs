using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories
{
    public class RepositoryHistoricalClimatology
    {
        private WebAPIForecast Client { get; set; }

        public RepositoryHistoricalClimatology(string root)
        {
            Client = new WebAPIForecast(root);
        }

        /// <summary>
        /// Method that gets the forecast information
        /// </summary>
        /// <param name="ws">Weather station ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<HistoricalClimatology>> SearchAsync(string ws)
        {
            IEnumerable<HistoricalClimatology> answer = await Client.GetHistoricalClimatologyAsync(ws);
            return answer;
        }
    }
}
