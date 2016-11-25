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
    /// This class allow to get information about historical climatic collection
    /// </summary>
    public class HistoricalClimaticFactory:FactoryDB<HistoricalClimatic>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public HistoricalClimaticFactory(IMongoDatabase database): base(database, LogEntity.hs_historical_climatic)
        {

        }

        public async override Task<bool> updateAsync(HistoricalClimatic entity, HistoricalClimatic newEntity)
        {
            throw new NotImplementedException();
        }

        public async override Task<bool> deleteAsync(HistoricalClimatic entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<HistoricalClimatic> insertAsync(HistoricalClimatic entity)
        {
            throw new NotImplementedException();
        }
    }
}
