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
using System.IO;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

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
        public HomeController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) :
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
                // type == 1 is for the configuration files
                if (type == 1)
                    path = configurationPath + file;

                if (System.IO.File.Exists(path))
                {
                    Stream file_temp = new FileStream(path, FileMode.Open, FileAccess.Read);
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

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            string url = System.Web.HttpUtility.UrlPathEncode(returnUrl);
            return Redirect(url);
        }
    }
}
