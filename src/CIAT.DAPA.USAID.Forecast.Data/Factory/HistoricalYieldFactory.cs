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

        /// <summary>
        /// Method that return all registers in the database 
        /// by the weather stations
        /// </summary>
        /// <param name="ws">Array of the weather stations ids</param>
        /// <returns>List of the historical yield data</returns>
        public async virtual Task<List<HistoricalYield>> byWeatherStationsAsync(ObjectId[] ws)
        {
            // Filter all entities available.
            var query = from hc in collection.AsQueryable()
                        where ws.Contains(hc.weather_station)
                        select hc;
            return query.ToList();
        }
    }
}
