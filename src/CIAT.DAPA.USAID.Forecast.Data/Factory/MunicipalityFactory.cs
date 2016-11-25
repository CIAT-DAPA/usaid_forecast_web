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

        }

        public async override Task<bool> updateAsync(Municipality entity, Municipality newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<Municipality>.Filter.Eq("_id", entity.id), newEntity);
            return true;
        }

        public async override Task<bool> deleteAsync(Municipality entity)
        {
            await collection.UpdateOneAsync(Builders<Municipality>.Filter.Eq("_id", entity.id), Builders<Municipality>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return true;
        }

        public async override Task<Municipality> insertAsync(Municipality entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }
    }
}
