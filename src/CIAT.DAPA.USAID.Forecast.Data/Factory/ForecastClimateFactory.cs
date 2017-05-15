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

        /// <summary>
        /// Method that return all records about climate of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastClimate>> byForecastAsync(ObjectId forecast)
        {
            var builder = Builders<ForecastClimate>.Filter;
            var filter = builder.Eq("forecast", forecast);
            return await collection.Find(filter).ToListAsync<ForecastClimate>();
        }

        /// <summary>
        /// Method that return all records about climate of the forecast by weather station
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastClimate>> byForecastAndWeatherStationAsync(ObjectId forecast, ObjectId[] ws)
        {
            var query = from fc in collection.AsQueryable()
                        where ws.Contains(fc.weather_station) && fc.forecast == forecast
                        select fc;
            return query;
        }
    }
}
