using CIAT.DAPA.USAID.Forecast.Data.Factory;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Database
{
    public class ForecastDB
    {
        /// <summary>
        /// Get the name of the database in mongo
        /// </summary>
        public const string DATABASE_NAME = "forecast_db";

        /// <summary>
        /// Get or set the client connection to mongo db 
        /// </summary>
        private MongoClient client { get; set; }
        /// <summary>
        /// Get or set the database from mongo
        /// </summary>
        private IMongoDatabase db { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private ConventionPack convention { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="connection">Connection string to communicate with mongo database</param>
        public ForecastDB(string connection)
        {
            client = new MongoClient(connection);
            db = client.GetDatabase(DATABASE_NAME);
            // Set the convetion for all fields of the mongo database
            convention = new ConventionPack();
            convention.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register("camelCase", convention, t => true);
        }

        public LocalityFactory locality { get { return new LocalityFactory(db); } }
    }
}
