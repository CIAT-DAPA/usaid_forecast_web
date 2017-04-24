using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    public class UserFactory : FactoryDB<User>
    {
        /// <summary>
        /// Get or set de manager user
        /// </summary>
        public UserManager<User> manager { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public UserFactory(IMongoDatabase database) : base(database, LogEntity.users)
        {
        }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        /// <param name="manager">Manager of web application</param>
        public UserFactory(IMongoDatabase database, UserManager<User> manager) : base(database, LogEntity.users)
        {
            this.manager = manager;
        }

        public override Task<bool> deleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<User> insertAsync(User entity)
        {
            var result = await manager.CreateAsync(entity, entity.password);
            return result.Succeeded ? await byIdAsync(entity.UserName) : null;
        }

        public override Task<bool> updateAsync(User entity, User newEntity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that search if exist a user by its email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>True if it exists, otherwise false</returns>
        public async Task<bool> existByEmailAsync(string email)
        {
            return (await manager.FindByEmailAsync(email)) != null;
        }

        /// <summary>
        /// Method that search a record in the database by its id
        /// </summary>
        /// <param name="id">User name</param>
        /// <returns>Entity if the database found some record, otherwise null</returns>
        public async override Task<User> byIdAsync(string id)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.Eq("UserName", id);
            var results = await collection.Find(filter).ToListAsync<User>();
            return results.FirstOrDefault();
        }
    }
}
