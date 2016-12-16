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
    /// This class allow to get information about historical yield collection
    /// </summary>
    public class HistoricalYieldFactory: FactoryDB<HistoricalYield>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public HistoricalYieldFactory(IMongoDatabase database): base(database, LogEntity.hs_historical_yield)
        {

        }

        public async override Task<bool> updateAsync(HistoricalYield entity, HistoricalYield newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<HistoricalYield>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(HistoricalYield entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<HistoricalYield> insertAsync(HistoricalYield entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }
    }
}
