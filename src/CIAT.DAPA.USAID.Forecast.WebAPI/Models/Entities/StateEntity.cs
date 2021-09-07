using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    /// <summary>
    /// This class has all attributes that are allowed search by the API
    /// about state entity from the database
    /// </summary>
    public class StateEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public CountryEntity country { get; set; }
        public List<MunicipalityEntity> municipalities { get; set; }
    }
}
