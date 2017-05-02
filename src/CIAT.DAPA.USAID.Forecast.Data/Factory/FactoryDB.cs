using CIAT.DAPA.USAID.Forecast.Data.Enums;
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
        public IMongoCollection<T> collection { get; private set; }        

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected</param>
        /// <param name="name">Name of the collection</param>
        public FactoryDB(IMongoDatabase database, LogEntity name)
        {
            db = database;
            name_collection = Enum.GetName(typeof(LogEntity), name);
            collection = db.GetCollection<T>(name_collection);
        }

        /// <summary>
        /// Method that add new entity to the database
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <returns>Entity with new Object ID</returns>
        public abstract Task<T> insertAsync(T entity);       
        
        /// <summary>
        /// Method that delete one entity in the database.
        /// The register is deleted logically, in another words the field enable
        /// is updated to false
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <returns>True if the register has been deleted, otherwise false</returns>      
        public abstract Task<bool> deleteAsync(T entity);

        /// <summary>
        /// Method that replace one entity in the database.
        /// The reokaced record is searching by its id.
        /// </summary>
        /// <param name="entity">Current entity</param>
        /// <param name="newEntity">New entity</param>
        /// <returns>True if the register has been replaced, otherwise false</returns>
        public abstract Task<bool> updateAsync(T entity, T newEntity);

        /// <summary>
        /// Method that return all registers enable in the database
        /// </summary>
        /// <returns>List of entity</returns>
        public async virtual Task<List<T>> listEnableAsync()
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("track.enable", true);
            return await collection.Find(filter).ToListAsync<T>();
        }

        /// <summary>
        /// Method that search a record in the database by its id
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <returns>Entity if the database found some record, otherwise null</returns>
        public async virtual Task<T> byIdAsync(string id)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id",new ObjectId(id));
            var results =  await collection.Find(filter).ToListAsync<T>();
            return results.FirstOrDefault();
        }
    }
}
