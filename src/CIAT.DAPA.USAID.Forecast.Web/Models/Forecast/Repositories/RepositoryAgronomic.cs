using CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Web.Models.Forecast.Repositories
{
    public class RepositoryAgronomic
    {
        private WebAPIForecast Client { get; set; }

        public RepositoryAgronomic(string root, string id_country)
        {
            Client = new WebAPIForecast(root, id_country);
        }

        /// <summary>
        /// Method that gets the configuration of crops
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Agronomic>> ListAsync()
        {
            IEnumerable<Agronomic> answer = await Client.GetAgronomicAsync();
            return answer;
        }
    }
}
