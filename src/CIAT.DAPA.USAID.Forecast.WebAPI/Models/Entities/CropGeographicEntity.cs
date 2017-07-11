using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    /// <summary>
    /// This class has all attributes that are allowed search by the API
    /// about crop geographic entity
    /// </summary>
    public class CropGeographicEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<StateEntity> states { get; set; }
    }
}
