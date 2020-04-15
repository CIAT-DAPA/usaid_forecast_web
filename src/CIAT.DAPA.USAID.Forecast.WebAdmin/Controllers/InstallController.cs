using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Account;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Install;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    public class InstallController : WebAdminBaseController
    {

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public InstallController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, 
            IEmailSender emailSender) : base(settings, LogEntity.users, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }


        // GET: /Install/Index
        [HttpGet]
        public IActionResult Index()
        {
            if (installed)
                return RedirectToAction("Index", "Home");
            return View();
        }


        // POST: /Install/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(InstallViewModel model)
        {
            if (installed)
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                await registerRoles();
                if(await registerUserAsync(model.Email, model.Password, Role.ROLE_ADMIN))
                    return RedirectToAction("Installed");
                else
                    return View(model);
            }
                
            return View(model);
        }

        // GET: /Install/Installed
        [HttpGet]
        public IActionResult Installed()
        {
            if (installed)
                return RedirectToAction("Index", "Home");
            return View();
        }

    }
}
