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
            var result = await collection.ReplaceOneAsync(Builders<Municipality>.Filter.Eq("_id", entity.id), newEntity);
            return true;
        }

        public async override Task<bool> deleteAsync(Municipality entity)
        {
            await collection.UpdateOneAsync(Builders<Municipality>.Filter.Eq("_id", entity.id), Builders<Municipality>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return true;
        }
    }
}
