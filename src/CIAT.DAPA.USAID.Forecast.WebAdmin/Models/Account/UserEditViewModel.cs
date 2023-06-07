using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Account
{
    public class UserEditViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        /*[Required]
        [Display(Name = "Role")]
        public List<string> Role { get; set; }*/

        [Required]
        [Display(Name = "Active")]
        public bool LockoutEnabled { get; set; }
        [Required]
        [Display(Name = "Countries")]
        public List<string> Countries { get; set; }
    }
}
