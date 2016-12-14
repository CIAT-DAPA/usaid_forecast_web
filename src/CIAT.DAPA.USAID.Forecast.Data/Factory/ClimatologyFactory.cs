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
    /// This class allow to get information about climatology collection
    /// </summary>
    public class ClimatologyFactory : FactoryDB<Climatology>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ClimatologyFactory(IMongoDatabase database): base(database, LogEntity.hs_climatology)
        {

        }
        
        public async override Task<bool> updateAsync(Climatology entity, Climatology newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<Climatology>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(Climatology entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<Climatology> insertAsync(Climatology entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method tat search a record by the weather station in the database
        /// </summary>
        /// <param name="ws">Id weather station</param>
        /// <returns>Climatology</returns>
        public async virtual Task<Climatology> byWeatherStationAsync(ObjectId ws)
        {
            var builder = Builders<Climatology>.Filter;
            var filter = builder.Eq("weather_station", ws);
            var results = await collection.Find(filter).ToListAsync<Climatology>();
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Method that return all registers in the database 
        /// by the weather stations
        /// </summary>
        /// <param name="ws">Array of the weather stations ids</param>
        /// <returns>List of the climatology</returns>
        public async virtual Task<List<Climatology>> byWeatherStationsAsync(ObjectId[] ws)
        {
            // Filter all entities available.
            var query = from cl in collection.AsQueryable()
                        where ws.Contains(cl.weather_station)
                        select cl;
            return query.ToList();
        }
    }
}
