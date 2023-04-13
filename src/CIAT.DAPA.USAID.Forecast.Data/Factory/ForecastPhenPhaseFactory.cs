using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    /// <summary>
    /// This class allow to get information about phenological phases forecast collection
    /// </summary>
    public class ForecastPhenPhaseFactory : FactoryDB<ForecastPhenPhase>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ForecastPhenPhaseFactory(IMongoDatabase database): base(database, LogEntity.fs_forecast_phen_phase)
        {
            var indexKeys = Builders<ForecastPhenPhase>.IndexKeys.Combine(
                            Builders<ForecastPhenPhase>.IndexKeys.Ascending(x => x.forecast),
                            Builders<ForecastPhenPhase>.IndexKeys.Ascending(x => x.ws),
                            Builders<ForecastPhenPhase>.IndexKeys.Ascending(x => x.soil),
                            Builders<ForecastPhenPhase>.IndexKeys.Ascending(x => x.cultivar));
            var indexOptions = new CreateIndexOptions { Unique = false }; // if you want the index is unique
            var indexModel = new CreateIndexModel<ForecastPhenPhase>(indexKeys, indexOptions);
            collection.Indexes.CreateOne(indexModel);
        }

        public async override Task<bool> updateAsync(ForecastPhenPhase entity, ForecastPhenPhase newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<ForecastPhenPhase>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(ForecastPhenPhase entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<ForecastPhenPhase> insertAsync(ForecastPhenPhase entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return all records about phenological phases of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastPhenPhase>> byForecastAsync(ObjectId forecast)
        {
            var builder = Builders<ForecastPhenPhase>.Filter;
            var filter = builder.Eq("forecast", forecast);
            return await collection.Find(filter).ToListAsync<ForecastPhenPhase>();
        }

        /// <summary>
        /// Method that return all records about phenological phases of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastPhenPhase>> byForecastAndWeatherStationAsync(ObjectId forecast, ObjectId[] ws)
        {
            var query = from fy in collection.AsQueryable()
                        where ws.Contains(fy.ws) && fy.forecast == forecast
                        select fy;
            return query;
        }
        /// <summary>
        /// Method that return all records about phenological phases of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast phenological phases</returns>
        public async Task<IEnumerable<ForecastPhenPhase>> byForecastAndWeatherStationExceedanceAsync(List<ObjectId> forecast, ObjectId[] ws)
        {
            var query = from fy in collection.AsQueryable()
                        where ws.Contains(fy.ws) && forecast.Contains(fy.forecast)
                        select fy;
            return query;
        }

        public async Task<IEnumerable<ForecastPhenPhase>> byIndexAsync(ObjectId forecast, ObjectId cultivar, ObjectId ws, ObjectId soil)
        {
            var builder = Builders<ForecastPhenPhase>.Filter;
            var filter = builder.And(builder.Eq(x => x.forecast, forecast), builder.Eq(x => x.ws, ws), builder.Eq(x => x.soil, soil), builder.Eq(x => x.cultivar, cultivar));
            var query = await collection.Find(filter).ToListAsync<ForecastPhenPhase>();
            return query;
        }
    }
}
