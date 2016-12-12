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
        /// Method that search all information available about geographic data to the end user and fixed all them into a comprensible data structure
        /// </summary>
        /// <returns>List of LocationView (it has information about states, municipalities and weather stations)</returns>
        public async Task<IEnumerable<LocationView>> listLocationVisibleAsync()
        {
            // Filter all entities available and visible to return.
            // For can say that a entity is available it should has the field track.enable in true
            var states = db.GetCollection<State>(Enum.GetName(typeof(LogEntity), LogEntity.lc_state))
                                .AsQueryable().Where(f => f.track.enable).ToList();
            var municipalities = db.GetCollection<Municipality>(Enum.GetName(typeof(LogEntity), LogEntity.lc_municipality))
                                .AsQueryable().Where(f => f.track.enable && f.visible).ToList();
            var weatherstations = db.GetCollection<WeatherStation>(Enum.GetName(typeof(LogEntity), LogEntity.lc_weather_station))
                                .AsQueryable().Where(f => f.track.enable && f.visible).ToList();
            // Join all data and groups the data by the state
            var query = from s in states
                        join m in municipalities on s.id equals m.state
                        join w in weatherstations on m.id equals w.municipality
                        group new { s, m, w } by m.state into joined
                        select joined.Select(p => new LocationView()
                        {
                            st_id = p.s.id.ToString(),
                            st_name = p.s.name,
                            st_country = p.s.country.name,
                            mn_id = p.m.id.ToString(),
                            mn_name = p.m.name,
                            ws_id = p.w.id.ToString(),
                            ws_name = p.w.name,
                            ws_origin = p.w.origin,
                            ws_ext = p.w.ext_id,
                            ws_lat = p.w.latitude,
                            ws_lon = p.w.longitude
                        });
            return query.FirstOrDefault();
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
                      .Select(p => new CultivarView() { id = p.id.ToString(), name = p.name, rainfed = p.rainfed }),
                    // Filter only the soils for the crop
                    soils = soils.Where(p => p.crop == c.id).OrderByDescending(p => p.order)
                      .Select(p => new SoilView() { id = p.id.ToString(), name = p.name })
                });
            return query;
        }
        /*
        public async Task<IEnumerable<ClimatologyView>> climatologyByStations(ObjectId[] ws_ids)
        {
            List<ClimatologyView> query = new List<ClimatologyView>();
            // Filter the weather stations available, visible and listed by their id
            var weatherstations = db.GetCollection<WeatherStation>(Enum.GetName(typeof(LogEntity), LogEntity.lc_weather_station))
                                        .AsQueryable().Where(f => f.track.enable && f.visible && ws_ids.Contains(f.id)).ToList();
            var climatology = db.GetCollection<Climatology>(Enum.GetName(typeof(LogEntity), LogEntity.hs_climatology));
            foreach (WeatherStation ws in weatherstations)
            {
                query.Add(new ClimatologyView()
                {
                    ws_id = ws.id.ToString(),
                    data = climatology.AsQueryable()
                                .Where(p => p.weather_station == ws.id)
                                .Select(p => new MonthlyClimateDataView()
                                {
                                    month = p.monthly_data.month,
                                    values = p.monthly_data.data
                                })
                });
            }
            return query;
        }*/
    }
}
