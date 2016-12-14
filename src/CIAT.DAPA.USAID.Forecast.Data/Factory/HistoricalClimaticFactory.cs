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
    /// This class allow to get information about historical climatic collection
    /// </summary>
    public class HistoricalClimaticFactory : FactoryDB<HistoricalClimatic>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public HistoricalClimaticFactory(IMongoDatabase database) : base(database, LogEntity.hs_historical_climatic)
        {

        }

        public async override Task<bool> updateAsync(HistoricalClimatic entity, HistoricalClimatic newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<HistoricalClimatic>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(HistoricalClimatic entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<HistoricalClimatic> insertAsync(HistoricalClimatic entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method tat search a record by the year and weather station in the database
        /// </summary>
        /// <param name="year">Year to search</param>
        /// <param name="ws">Id weather station</param>
        /// <returns>Historical data</returns>
        public async virtual Task<HistoricalClimatic> byYearWeatherStationAsync(int year, ObjectId ws)
        {
            var builder = Builders<HistoricalClimatic>.Filter;
            var filter = builder.Eq("year", year) & builder.Eq("weather_station", ws);
            var results = await collection.Find(filter).ToListAsync<HistoricalClimatic>();
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Method that return all registers in the database 
        /// by the weather stations
        /// </summary>
        /// <param name="ws">Array of the weather stations ids</param>
        /// <returns>List of the historical climatic data</returns>
        public async virtual Task<List<HistoricalClimatic>> byWeatherStationsAsync(ObjectId[] ws)
        {
            // Filter all entities available.
            var query = from hc in collection.AsQueryable()
                        where ws.Contains(hc.weather_station)
                        select hc;
            return query.ToList();
        }
    }
}
