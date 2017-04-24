﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Role
    {
        /// <summary>
        /// Get the list of the roles in the web platform
        /// </summary>
        public static readonly string[] ROLES_PLATFORM = { "admin", "climatologist", "improver", "tech" };

        /// <summary>
        /// Method Construct
        /// </summary>
        public Role():base()
        {   
        }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="roleName">Role name</param>
        public Role(string roleName):base(roleName)
        {

        }
    }
}
