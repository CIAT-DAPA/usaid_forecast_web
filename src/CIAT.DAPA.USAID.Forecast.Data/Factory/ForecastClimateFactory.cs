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
    /// This class allow to get information about climate forecast collection
    /// </summary>
    public class ForecastClimateFactory: FactoryDB<ForecastClimate>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ForecastClimateFactory(IMongoDatabase database): base(database, LogEntity.fs_forecast_climate)
        {

        }

        public async override Task<bool> updateAsync(ForecastClimate entity, ForecastClimate newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<ForecastClimate>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(ForecastClimate entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<ForecastClimate> insertAsync(ForecastClimate entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }
    }
}
