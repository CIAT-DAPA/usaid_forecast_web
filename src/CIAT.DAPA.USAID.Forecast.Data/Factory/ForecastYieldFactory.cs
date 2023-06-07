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
            var indexKeys = Builders<ForecastYield>.IndexKeys.Combine(
                            Builders<ForecastYield>.IndexKeys.Ascending(x => x.forecast),
                            Builders<ForecastYield>.IndexKeys.Ascending(x => x.cultivar));
            var indexOptions = new CreateIndexOptions { Unique = false }; // if you want the index is unique
            var indexModel = new CreateIndexModel<ForecastYield>(indexKeys, indexOptions);
            collection.Indexes.CreateOne(indexModel);
        }

        public async override Task<bool> updateAsync(ForecastYield entity, ForecastYield newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<ForecastYield>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(ForecastYield entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<ForecastYield> insertAsync(ForecastYield entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return all records about climate of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastYield>> byForecastAsync(ObjectId forecast)
        {
            var builder = Builders<ForecastYield>.Filter;
            var filter = builder.Eq("forecast", forecast);
            return await collection.Find(filter).ToListAsync<ForecastYield>();
        }

        /// <summary>
        /// Method that return all records about climate of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastYield>> byForecastAndWeatherStationAsync(ObjectId forecast, ObjectId[] ws)
        {
            var query = from fy in collection.AsQueryable()
                        where ws.Contains(fy.weather_station) && fy.forecast == forecast
                        select fy;
            return query;
        }

        public async Task<IEnumerable<ForecastYield>> byForecastCul(ObjectId forecast, IEnumerable<ObjectId> cultivar)
        {
            var builder = Builders<ForecastYield>.Filter;
            var filter = builder.And(builder.Eq(x => x.forecast, forecast), builder.In(x => x.cultivar, cultivar));
            var query = await collection.Find(filter).ToListAsync<ForecastYield>();
            return query;
        }
        /// <summary>
        /// Method that return all records about climate of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastYield>> byForecastAndWeatherStationExceedanceAsync(List<ObjectId> forecast, ObjectId[] ws)
        {
            var query = from fy in collection.AsQueryable()
                        where ws.Contains(fy.weather_station) && forecast.Contains(fy.forecast)
                        select fy;
            return query;
        }
    }
}
