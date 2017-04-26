﻿using CIAT.DAPA.USAID.Forecast.Data.Enums;
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
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public UserFactory(IMongoDatabase database) : base(database, LogEntity.users)
        {
        }

        public override Task<bool> deleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public override Task<User> insertAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> updateAsync(User entity, User newEntity)
        {
            throw new NotImplementedException();
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
