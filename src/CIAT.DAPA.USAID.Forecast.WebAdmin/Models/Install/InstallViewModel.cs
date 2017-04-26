﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Install
{
    public class InstallViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe ser al menos {2} y máximo {1} caracteres de longitud.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "El password y la confirmación del password no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Notificaciones")]
        public string NotifyEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe ser al menos {2} y máximo {1} caracteres de longitud.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password de cuenta de notificaciones")]
        public string NotifyPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("NotifyPassword", ErrorMessage = "El password y la confirmación del password no coinciden.")]
        public string NotifyConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Puerto")]
        public int NotifyPort { get; set; }

        [Required]
        [Display(Name = "Seguridad Ssl")]
        public bool NotifySsl { get; set; }
    }
}
