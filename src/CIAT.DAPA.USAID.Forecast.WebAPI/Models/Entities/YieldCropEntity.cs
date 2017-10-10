using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class YieldCropEntity
    {
        
        public ObjectId soil { get; set; }
        
        public ObjectId cultivar { get; set; }

        public DateTime start { get; set; }
        
        public DateTime end { get; set; }
        
        public IEnumerable<YieldData> data { get; set; }
    }
}
