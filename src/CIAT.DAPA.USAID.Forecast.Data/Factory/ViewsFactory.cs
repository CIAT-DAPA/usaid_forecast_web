using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.Data.Views;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    /// <summary>
    /// This class has the queries that represents a view of the data in the database.
    /// Those queries are part of the bussines logic
    /// </summary>
    public class ViewsFactory
    {
        /// <summary>
        /// Get or set the mongo database
        /// </summary>
        protected IMongoDatabase db { get; private set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected</param>
        public ViewsFactory(IMongoDatabase database)
        {
            db = database;
        }

        /// <summary>
        /// Method that search all information available about agronomic data to the end user and fixed all them into a comprensible data structure
        /// </summary>
        /// <returns>List of AgronomicView (it has information about crops, cultivars and soils)</returns>
        public async Task<IEnumerable<AgronomicView>> listAgronomicDataAsync()
        {
            List<AgronomicView> query = new List<AgronomicView>();
            // Filter all entities available.
            // For can say that a entity is available it should has the field track.enable in true
            var crops = db.GetCollection<Crop>(Enum.GetName(typeof(LogEntity), LogEntity.cp_crop))
                            .AsQueryable().Where(f => f.track.enable).ToList();
            var cultivars = db.GetCollection<Cultivar>(Enum.GetName(typeof(LogEntity), LogEntity.cp_cultivar))
                            .AsQueryable().Where(f => f.track.enable).ToList();
            var soils = db.GetCollection<Soil>(Enum.GetName(typeof(LogEntity), LogEntity.cp_soil))
                            .AsQueryable().Where(f => f.track.enable).ToList();
            // Filter the data by every crop 
            foreach (var c in crops)
                query.Add(new AgronomicView()
                {
                    cp_id = c.id.ToString(),
                    cp_name = c.name,
                    // Filter only the cultivars for the crop
                    cultivars = cultivars.Where(p => p.crop == c.id).OrderByDescending(p => p.order)
                      .Select(p => new CultivarView() { id = p.id.ToString(), name = p.name, rainfed = p.rainfed, national = p.national, country_id = p.country.ToString() }),
                    // Filter only the soils for the crop
                    soils = soils.Where(p => p.crop == c.id).OrderByDescending(p => p.order)
                      .Select(p => new SoilView() { id = p.id.ToString(), name = p.name, country_id = p.country.ToString() })
                });
            return query;
        }
    }
}
