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
    /// This class allow to get information about municipality collection
    /// </summary>
    public class MunicipalityFactory: FactoryDB<Municipality>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public MunicipalityFactory(IMongoDatabase database) : base(database, LogEntity.lc_municipality)
        {
            // foreign index on state id
            var indexKeys = Builders<Municipality>.IndexKeys.Ascending(x => x.state);
            var indexOptions = new CreateIndexOptions { Unique = false };
            var indexModel = new CreateIndexModel<Municipality>(indexKeys, indexOptions);
            collection.Indexes.CreateOne(indexModel);
        }

        public async override Task<bool> updateAsync(Municipality entity, Municipality newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<Municipality>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(Municipality entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Municipality>.Filter.Eq("_id", entity.id), Builders<Municipality>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Municipality> insertAsync(Municipality entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }
                
        /// <summary>
        /// Method that return all registers enable in the database
        /// by the state
        /// </summary>
        /// <param name="state">Id of state</param>
        /// <returns>List of municipalities</returns>
        public async virtual Task<List<Municipality>> listEnableByStateAsync(ObjectId state)
        {
            var builder = Builders<Municipality>.Filter;
            var filter = builder.Eq("track.enable", true) & builder.Eq("state", state);
            return await collection.Find(filter).ToListAsync<Municipality>();
        }

        /// <summary>
        /// Method that return all registers enable and visible in the database
        /// </summary>
        /// <returns>List of municipalities</returns>
        public async virtual Task<List<Municipality>> listEnableVisibleAsync()
        {
            var builder = Builders<Municipality>.Filter;
            var filter = builder.Eq("track.enable", true) & builder.Eq("visible", true);
            return await collection.Find(filter).ToListAsync<Municipality>();
        }


        /// <summary>
        /// Method that return all registers enable and visible in the database for provided state ids
        /// </summary>
        /// <returns>List of municipalities</returns>
        public async virtual Task<List<Municipality>> listEnableVisibleByStatesAsync(ObjectId[] states)
        {

            // Filter all entities available.
            var query = from m in collection.AsQueryable()
                        where m.track.enable && m.visible && states.Contains(m.state)
                        select m;
            return query.ToList();

        }

        /// <summary>
        /// Method that search a record in the database by its name
        /// </summary>
        /// <param name="name">Name of municipality</param>
        /// <returns>Entity if the database found some record, otherwise null</returns>
        public async virtual Task<Municipality> byNameAsync(string name)
        {
            var builder = Builders<Municipality>.Filter;
            var filter = builder.Eq("track.enable", true) & builder.Eq("name", name);
            var results = await collection.Find(filter).ToListAsync<Municipality>();
            return results.FirstOrDefault();
        }

        public async Task<List<Municipality>> listAllAsync()
        {
            return await collection.Find("{}").ToListAsync<Municipality>();
        }
    }
}
