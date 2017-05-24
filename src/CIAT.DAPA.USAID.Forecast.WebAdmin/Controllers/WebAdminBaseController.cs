using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Factory;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Account;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    public class WebAdminBaseController : Controller
    {
        /// <summary>
        /// Get or set the hosting enviroment
        /// </summary>
        protected IHostingEnvironment hostingEnvironment { get; set; }
        /// <summary>
        /// Get or set object to connect with database
        /// </summary>
        protected ForecastDB db { get; set; }
        /// <summary>
        /// Get or set object to write the events occurring on the website 
        /// </summary>
        protected CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools.Log log { get; set; }
        /// <summary>
        /// List of the entities affected
        /// </summary>
        protected List<LogEntity> entities { get; set; }
        /// <summary>
        /// Path where the imports files are located
        /// </summary>
        protected string importPath { get; set; }
        /// <summary>
        /// Path where the configuration files are located
        /// </summary>
        protected string configurationPath { get; set; }
        /// <summary>
        /// Get if the application was installed or not
        /// </summary>
        protected bool installed { get; private set; }
        /// <summary>
        /// Get the user manager
        /// </summary>
        protected UserManager<IdentityUser> managerUser { get; private set; }
        /// <summary>
        /// Get the role manager
        /// </summary>
        protected RoleManager<IdentityRole> managerRole { get; private set; }
        /// <summary>
        /// Get the signin manager
        /// </summary>
        protected SignInManager<IdentityUser> managerSignIn { get; private set; }
        /// <summary>
        /// Get or set email sender
        /// </summary>
        protected IEmailSender notifyEmail;


        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="entity">List of entities affected</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public WebAdminBaseController(IOptions<Settings> settings, LogEntity entity, IHostingEnvironment environment) : base()
        {
            init(settings, entity, environment);
        }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="entity">List of entities affected</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public WebAdminBaseController(IOptions<Settings> settings, LogEntity entity, IHostingEnvironment environment, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender) : base()
        {
            init(settings, entity, environment);
            managerUser = userManager;
            managerSignIn = signInManager;
            managerRole = roleManager;
            notifyEmail = emailSender;
        }

        /// <summary>
        /// Method that set the initial values
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="entity">List of entities affected</param>
        /// <param name="environment">Host Enviroment</param>
        private void init(IOptions<Settings> settings, LogEntity entity, IHostingEnvironment environment)
        {
            entities = new List<LogEntity>() { entity };
            hostingEnvironment = environment;
            db = new ForecastDB(settings.Value.ConnectionString, settings.Value.Database);
            log = new CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools.Log(hostingEnvironment.ContentRootPath + settings.Value.LogPath, db.logAdministrative);
            importPath = hostingEnvironment.ContentRootPath + settings.Value.ImportPath;
            configurationPath = hostingEnvironment.ContentRootPath + settings.Value.ConfigurationPath;
            installed = settings.Value.Installed;
        }

        /// <summary>
        /// Method that write event in the log
        /// </summary>
        /// <param name="content">Description event</param>
        /// <param name="e">Type event</param>
        /// <param name="entities_affected">List of the entities affected</param>
        public async Task writeEventAsync(string content, LogEvent e, List<LogEntity> entities_affected)
        {
            var user = await GetCurrentUserAsync();
            log.writeAsync(content, entities_affected, e, user == null ? "anonymous" : user.Id.ToString());
        }

        /// <summary>
        /// Method that write event in the log
        /// </summary>
        /// <param name="content">Description event</param>
        /// <param name="e">Type event</param>
        public async Task writeEventAsync(string content, LogEvent e)
        {
            await writeEventAsync(content, e, entities);
        }

        /// <summary>
        /// Method that write an error event in the website
        /// </summary>
        /// <param name="ex">Exception</param>
        public async Task writeExceptionAsync(Exception ex)
        {
            await writeEventAsync(ex.Message + " - " + ex.StackTrace.ToString(), LogEvent.exc, entities);
        }

        /// <summary>
        /// Method that return a object id from a string
        /// </summary>
        /// <param name="id">String hash to convert in ObjectId</param>
        /// <returns>ObjectId</returns>
        protected ObjectId getId(string id)
        {
            return ForecastDB.parseId(id);
        }

        /// <summary>
        /// Method that return the current user
        /// </summary>
        /// <returns></returns>
        protected Task<IdentityUser> GetCurrentUserAsync()
        {
            try
            {
                return managerUser.GetUserAsync(HttpContext.User);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Method that register a user in the platform
        /// </summary>
        /// <param name="email">Email user</param>
        /// <param name="password">Password user</param>
        /// <param name="role">Role name</param>
        /// <returns></returns>
        protected async Task<bool> registerUserAsync(string email, string password, string role)
        {
            try
            {
                var user = new IdentityUser { UserName = email, Email = email };
                await writeEventAsync("Register user email: " + email + " role: " + role, LogEvent.rea);
                var result = await managerUser.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await writeEventAsync("Register user email: " + email + " - Created ", LogEvent.rea);
                    await managerUser.AddToRoleAsync(user, role);
                    // Send an email with this link
                    var code = await managerUser.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await notifyEmail.SendEmailAsync(email, "Confirmar cuenta",
                        $"<p style=\"text-align:justify;\">Estimado usuario<br/><br/>Para confirmar su cuenta por favor presione click en el siguiente <a href=\"{callbackUrl}\">link</a></p>");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Method that create all roles for the platform
        /// </summary>
        /// <returns>True if it creates all roles, otherwise false</returns>
        protected async Task<bool> registerRoles()
        {
            try
            {
                foreach (var role in Role.ROLES_PLATFORM)
                {
                    var r = await db.role.byNameAsync(role);
                    if (r == null)
                        await managerRole.CreateAsync(new IdentityRole(role));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
