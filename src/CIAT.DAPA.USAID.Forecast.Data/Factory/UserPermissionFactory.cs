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
    /// This class allow to get information about soils collection
    /// </summary>
    public class UserPermissionFactory : FactoryDB<UserPermission>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public UserPermissionFactory(IMongoDatabase database) : base(database, LogEntity.ad_user_permission)
        {

        }

        public async override Task<bool> updateAsync(UserPermission entity, UserPermission newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<UserPermission>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(UserPermission entity)
        {
            var result = await collection.UpdateOneAsync(Builders<UserPermission>.Filter.Eq("_id", entity.id), Builders<UserPermission>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<UserPermission> insertAsync(UserPermission entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }

        public async override Task<List<UserPermission>> listEnableAsync()
        {
            var builder = Builders<UserPermission>.Filter;
            var filter = builder.Eq("track.enable", true);
            return await collection.Find(filter).ToListAsync<UserPermission>();
        }

        /// <summary>
        /// Method that return all records about scenarios climate of the forecast by weather station
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<UserPermission> byUserAsync(string user)
        {
            var query = from usr in collection.AsQueryable()
                        where user == usr.user
                        select usr;
            return query.FirstOrDefault();
        }
    }
}
