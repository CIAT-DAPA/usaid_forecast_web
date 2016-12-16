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
        public string database_name { get; set; }

        /// <summary>
        /// Get or set the client connection to mongo db 
        /// </summary>
        private MongoClient client { get; set; }
        /// <summary>
        /// Get or set the database from mongo
        /// </summary>
        private IMongoDatabase db { get; set; }
        /// <summary>
        /// Get or set the convention to setup the collection in the database
        /// </summary>
        private ConventionPack convention { get; set; }

        /// <summary>
        /// Get or set the state entity in the database
        /// </summary>
        public StateFactory state { get; set; }
        /// <summary>
        /// Get or set the municipality entity in the database
        /// </summary>
        public MunicipalityFactory municipality { get; set; }
        /// <summary>
        /// Get or set the weather station entity in the database
        /// </summary>
        public WeatherStationFactory weatherStation { get; set; }
        /// <summary>
        /// Get or set the cultivar entity in the database
        /// </summary>
        public CultivarFactory cultivar { get; set; }
        // <summary>
        /// Get or set the cultivar entity in the database
        /// </summary>
        public SoilFactory soil { get; set; }
        /// <summary>
        /// Get or set the crop entity in the database
        /// </summary>
        public CropFactory crop { get; set; }
        /// <summary>
        /// Get or set the historical climatic entity in the database
        /// </summary>
        public HistoricalClimaticFactory historicalClimatic { get; set; }
        /// <summary>
        /// Get or set the historical yield entity in the database
        /// </summary>
        public HistoricalYieldFactory historicalYield { get; set; }
        /// <summary>
        /// Get or set the climatology entity in the database
        /// </summary>
        public ClimatologyFactory climatology { get; set; }
        /// <summary>
        /// Get or set the forecast entity in the database
        /// </summary>
        public ForecastFactory forecast { get; set; }
        // <summary>
        /// Get or set the forecast climate entity in the database
        /// </summary>
        public ForecastClimateFactory forecastClimate { get; set; }
        // <summary>
        /// Get or set the forecast yield entity in the database
        /// </summary>
        public ForecastYieldFactory forecastYield { get; set; }
        /// <summary>
        /// Get or set the log administrative entity in the database
        /// </summary>
        public LogAdministrativeFactory logAdministrative { get; set; }
        /// <summary>
        /// Get or set the log service entity in the database
        /// </summary>
        public LogServiceFactory logService { get; set; }
        /// <summary>
        /// Get or set the all views of the database. It has some complex queries to the database
        /// </summary>
        public ViewsFactory views { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="connection">Connection string to communicate with mongo database</param>
        /// <param name="name">Name of the database</param>
        public ForecastDB(string connection, string name)
        {
            // Set the configuration connect with the database
            database_name = name;
            client = new MongoClient(connection);
            db = client.GetDatabase(database_name);
            // Set the convetion for all fields of the mongo database
            convention = new ConventionPack();
            convention.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register("camelCase", convention, t => true);
            // Start the collections setup in the database
            init();
        }

        /// <summary>
        /// Method that setup all collection of the database with POCO
        /// </summary>
        public void init()
        {
            // entities
            logAdministrative = new LogAdministrativeFactory(db);
            logService = new LogServiceFactory(db);
            state = new StateFactory(db);
            municipality = new MunicipalityFactory(db);
            weatherStation = new WeatherStationFactory(db);
            crop = new CropFactory(db);
            cultivar = new CultivarFactory(db);
            soil = new SoilFactory(db);
            historicalClimatic = new HistoricalClimaticFactory(db);
            historicalYield = new HistoricalYieldFactory(db);
            climatology = new ClimatologyFactory(db);
            forecast = new ForecastFactory(db);
            forecastClimate = new ForecastClimateFactory(db);
            forecastYield = new ForecastYieldFactory(db);
            // views
            views = new ViewsFactory(db);
        }
    }
}
