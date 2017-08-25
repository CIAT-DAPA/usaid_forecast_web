using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Account
{
    public class ManageViewModel
    {
        [Required]
        [Display(Name = "Actual password")]
        [StringLength(100, ErrorMessage = "La {0} debe ser al menos {2} y máximo {1} caracteres de longitud.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [Display(Name = "Nuevo Password")]
        [StringLength(100, ErrorMessage = "La {0} debe ser al menos {2} y máximo {1} caracteres de longitud.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nuevo password")]
        [Compare("Password", ErrorMessage = "El password y la confirmación del password no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
