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
    /// This class allow to get information about scenario forecast collection
    /// </summary>
    public class ForecastScenarioFactory : FactoryDB<ForecastScenario>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ForecastScenarioFactory(IMongoDatabase database) : base(database, LogEntity.fs_forecast_scenario)
        {

        }

        public async override Task<bool> updateAsync(ForecastScenario entity, ForecastScenario newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<ForecastScenario>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(ForecastScenario entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<ForecastScenario> insertAsync(ForecastScenario entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return all records about scenarios climate of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastScenario>> byForecastAsync(ObjectId forecast)
        {
            var builder = Builders<ForecastScenario>.Filter;
            var filter = builder.Eq("forecast", forecast);
            return await collection.Find(filter).ToListAsync<ForecastScenario>();
        }

        /// <summary>
        /// Method that return all records about scenarios climate of the forecast by weather station
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<ForecastScenario>> byForecastAndWeatherStationAsync(ObjectId forecast, ObjectId[] ws)
        {
            var query = from fs in collection.AsQueryable()
                        where ws.Contains(fs.weather_station) && fs.forecast == forecast
                        select fs;
            return query;
        }
    }
}
