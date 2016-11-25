using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    /// <summary>
    /// This class allow to get information about yield forecast collection
    /// </summary>
    public class ForecastYieldFactory: FactoryDB<ForecastYield>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ForecastYieldFactory(IMongoDatabase database): base(database, LogEntity.fs_forecast_yield)
        {

        }

        public async override Task<bool> updateAsync(ForecastYield entity, ForecastYield newEntity)
        {
            throw new NotImplementedException();
        }

        public async override Task<bool> deleteAsync(ForecastYield entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<ForecastYield> insertAsync(ForecastYield entity)
        {
            throw new NotImplementedException();
        }
    }
}
