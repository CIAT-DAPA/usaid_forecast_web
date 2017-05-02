using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Hosting;
using CIAT.DAPA.USAID.Forecast.Data.Enums;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [AllowAnonymous]
    public class HomeController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public HomeController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment) : base(settings, LogEntity.users, hostingEnvironment)
        {
        }

        public IActionResult Index()
        {
            if (!installed)
                return RedirectToAction("Index", "Install");
            return View();
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
