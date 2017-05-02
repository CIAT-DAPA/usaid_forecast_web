using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    public class RoleFactory : FactoryDB<Role>
    {        

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public RoleFactory(IMongoDatabase database) : base(database, LogEntity.roles)
        {
        }
               

        public override Task<bool> deleteAsync(Role entity)
        {
            throw new NotImplementedException();
        }

        public override Task<Role> insertAsync(Role entity)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> updateAsync(Role entity, Role newEntity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that search a record in the database by its name
        /// </summary>
        /// <param name="name">Role name</param>
        /// <returns>Entity if the database found some record, otherwise null</returns>
        public async Task<Role> byNameAsync(string name)
        {
            var builder = Builders<Role>.Filter;
            var filter = builder.Eq("Name", name);
            var results = await collection.Find(filter).ToListAsync<Role>();
            return results.FirstOrDefault();
        }
    }
}
