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
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            newEntity.conf = entity.conf;
            var result = await collection.ReplaceOneAsync(Builders<State>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(State entity)
        {
            var result = await collection.UpdateOneAsync(Builders<State>.Filter.Eq("_id", entity.id), Builders<State>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<State> insertAsync(State entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            entity.conf = new List<ConfigurationCPT>();
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that add a new cpt configuration
        /// </summary>
        /// <param name="entity">State with the new configuration</param>
        /// <param name="conf">New configuration to add to the state</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> addConfigurationCPTAsync(State entity, ConfigurationCPT conf)
        {
            conf.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            List<ConfigurationCPT> allConf = entity.conf.ToList();
            allConf.Add(conf);
            entity.conf = allConf;
            var result = await collection.UpdateOneAsync(Builders<State>.Filter.Eq("_id", entity.id),
                Builders<State>.Update.Set("conf", entity.conf));
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Method that delete a new cpt configuration
        /// </summary>
        /// <param name="entity">State with the new configuration</param>
        /// <param name="quarter">Year quarter</param>
        /// <param name="cca">Canonical correlation</param>
        /// <param name="gamma">Use gamma transformation</param>
        /// <param name="x"># of x modes</param>
        /// <param name="y"># of y modes</param>        
        /// <param name="left_lat">Left latitude</param>
        /// <param name="left_lon">Left longitude</param>
        /// <param name="right_lat">Right latitude</param>
        /// <param name="right_lon">Right longitude</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> deleteConfigurationCPTAsync(State entity, Quarter quarter, int cca, bool gamma, int x, int y, DateTime register)
        {
            List<ConfigurationCPT> allConf = new List<ConfigurationCPT>();
            foreach (var c in entity.conf)
            {
                if (c.trimester == quarter && c.cca_mode == cca && c.gamma == gamma && c.x_mode == x && c.y_mode == y && c.track.register.ToString("yyyy-MM-dd HH:mm:ss") == register.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    c.track.updated = DateTime.Now;
                    c.track.enable = false;
                }
                allConf.Add(c);
            }
            entity.conf = allConf;
            var result = await collection.UpdateOneAsync(Builders<State>.Filter.Eq("_id", entity.id),
                Builders<State>.Update.Set("conf", entity.conf));
            return result.ModifiedCount > 0;
        }
        public async Task<List<State>> listAllAsync()
        {
            return await collection.Find("{}").ToListAsync<State>();
        }

        public async Task<List<State>> listByCountryAsync()
        {
            var id = "6140ae5af88b8ef0235987f3";
            var builder = Builders<State>.Filter;
            var filter = builder.Eq("country", new ObjectId(id));
            return await collection.Find(filter).ToListAsync<State>();
        }
    }
}
