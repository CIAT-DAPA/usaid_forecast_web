using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIAT.DAPA.USAID.Forecast.Data.Models;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    /// <summary>
    /// This class allow to get information about forecast collection
    /// </summary>
    public class ForecastFactory: FactoryDB<CIAT.DAPA.USAID.Forecast.Data.Models.Forecast>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ForecastFactory(IMongoDatabase database): base(database, LogEntity.fs_forecast)
        {

        }

        public async override Task<bool> updateAsync(CIAT.DAPA.USAID.Forecast.Data.Models.Forecast entity, CIAT.DAPA.USAID.Forecast.Data.Models.Forecast newEntity)
        {
            throw new NotImplementedException();
        }

        public async override Task<bool> deleteAsync(CIAT.DAPA.USAID.Forecast.Data.Models.Forecast entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<Models.Forecast> insertAsync(Models.Forecast entity)
        {
            throw new NotImplementedException();
        }
    }
}
