using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    public abstract class FactoryDB<T>
    {
        /// <summary>
        /// Get or set name of the collection
        /// </summary>
        protected string name_collection { get; private set; }
        /// <summary>
        /// Get or set the mongo database
        /// </summary>
        protected IMongoDatabase db { get; private set; }
        /// <summary>
        /// Get or set the collection of the database
        /// </summary>
        protected IMongoCollection<T> collection { get; private set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected</param>
        public FactoryDB(IMongoDatabase database)
        {
            db = database;
            name_collection = typeof(T).Name.ToLower();
            collection = db.GetCollection<T>(name_collection);
        }

        /// <summary>
        /// Method that add new entity to the database
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <returns>Entity with new Object ID</returns>
        public async Task<T> insertAsync(T entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that delete one entity in the database.
        /// The register is deleted logically, in another words the field enable
        /// is updated to false
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <returns>True if the register is deleted, otherwise false</returns>
        public async Task<bool> deleteAsync(T entity)
        {
            Type t = entity.GetType();
            PropertyInfo prop = t.GetRuntimeProperty("_id");
            var id = prop.GetValue(entity).ToString();
            await collection.UpdateOneAsync(Builders<T>.Filter.Eq("_id",id), Builders<T>.Update.Inc("enable", false));
            return true;
        }

        /// <summary>
        /// Method that return all registers enable in the database
        /// </summary>
        /// <returns>List of entity</returns>
        public async Task<List<T>> listEnableAsync()
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("enable", true);
            return await collection.Find(filter).ToListAsync<T>();
        }
        /// <summary>
        /// Method that search a record in the database by its id
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <returns>Entity if the database found some record, otherwise null</returns>
        public async Task<T> byIdAsync(string id)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id",new ObjectId(id));
            var results =  await collection.Find(filter).ToListAsync<T>();
            return results.FirstOrDefault();
        }
    }
}
