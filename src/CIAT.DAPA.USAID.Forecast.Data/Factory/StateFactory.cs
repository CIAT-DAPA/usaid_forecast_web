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
    /// This class allow to get information about states collection
    /// </summary>
    public class StateFactory : FactoryDB<State>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public StateFactory(IMongoDatabase database) : base(database, LogEntity.lc_state)
        {

        }
        
        public async override Task<bool> updateAsync(State entity, State newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<State>.Filter.Eq("_id", entity.id), newEntity);
            return true;
        }
                
        public async override Task<bool> deleteAsync(State entity)
        {
            var result = await collection.UpdateOneAsync(Builders<State>.Filter.Eq("_id",entity.id), Builders<State>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return true;
        }
    }
}
