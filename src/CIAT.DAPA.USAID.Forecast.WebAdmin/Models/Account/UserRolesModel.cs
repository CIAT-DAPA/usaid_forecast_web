using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Account
{
    public class UserRoles
    {
        public string id { get; set; }
        public List<string> listData { get; set; }
    }
}
