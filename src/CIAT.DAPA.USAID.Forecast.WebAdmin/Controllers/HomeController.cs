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
using System.IO;

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

        // GET: /Home/Download/
        [HttpGet]
        [Authorize(Roles = "ADMIN,IMPROVER,CLIMATOLOGIST,TECH")]
        public async Task<IActionResult> Download(string file, int type)
        {
            try
            {
                string path = string.Empty;
                if (type == 1)
                    path = configurationPath + file;

                if (System.IO.File.Exists(path))
                {
                    Stream file_temp = new FileStream(path, FileMode.Open);
                    return File(file_temp, "plain/txt", file);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return NotFound();
            }
        }
    }
}
