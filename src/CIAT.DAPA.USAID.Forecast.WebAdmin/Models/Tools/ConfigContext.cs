using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools
{
    public class ConfigContext
    {
        /// <summary>
        /// Get or set the connection with the database
        /// </summary>
        private ForecastDB db { get; set; }
        /// <summary>
        /// Get or set the admin user
        /// </summary>
        private string adminUser { get; set; }
        /// <summary>
        /// Get or set the admin user password
        /// </summary>
        private string adminPwd { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="cnn">Connection string</param>
        /// <param name="database">Database name</param>
        /// <param name="adminUser">User admin (email)</param>
        /// <param name="adminPwd">Password admin. Remember the</param>
        public ConfigContext(string cnn,string database, string adminUser, string adminPwd)
        {
            db = new ForecastDB(cnn, database);
            this.adminUser = adminUser;
            this.adminPwd = adminPwd;
        }

        /// <summary>
        /// Method to configure the website administration with the users and roles
        /// </summary>
        /// <returns>Task</returns>
        public async Task CreateRolesAndUserAsync()
        {
            try
            {
                db.role.manager = new UserManager<User>(new UserStore<User>(db.user.collection));
                // The following cicle search if the roles were created in the database.
                // If the role wasn't created, it will register in the database
                foreach (string role in Role.ROLES_PLATFORM)
                    // The role doesn't exist
                    if (await db.role.byNameAsync(role) == null)
                        await db.role.insertAsync(new Role(role));

                // Create user admin
                bool exist = await db.user.existByEmailAsync(adminUser);
                if (!exist)
                    await db.user.insertAsync(new User() { UserName = adminUser, Email = adminUser, password = adminPwd });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
