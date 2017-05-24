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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;

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
        public HomeController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender) : 
            base(settings, LogEntity.users, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
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
