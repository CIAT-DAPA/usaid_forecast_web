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
    /// This class allow to get information about cultivars collection
    /// </summary>
    public class CultivarFactory: FactoryDB<Cultivar>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public CultivarFactory(IMongoDatabase database): base(database, LogEntity.cp_cultivar)
        {

        }

        public async override Task<bool> updateAsync(Cultivar entity, Cultivar newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<Cultivar>.Filter.Eq("_id", entity.id), newEntity);
            return true;
        }

        public async override Task<bool> deleteAsync(Cultivar entity)
        {
            await collection.UpdateOneAsync(Builders<Cultivar>.Filter.Eq("_id", entity.id), Builders<Cultivar>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return true;
        }
    }
}
