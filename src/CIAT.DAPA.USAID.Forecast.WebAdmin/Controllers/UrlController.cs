using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    [Authorize(Roles = "ADMIN,IMPROVER")]
    [Authorize]
    public class UrlController : WebAdminBaseController
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public UrlController(IOptions<Settings> settings, IHostingEnvironment hostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, IEmailSender emailSender) :
            base(settings, LogEntity.cp_url, hostingEnvironment, userManager, signInManager, roleManager, emailSender)
        {
        }

        private async Task<PermissionList> LoadEnableByPermissionAsync(string countrySelected = "")
        {
            UserPermission permission = await getPermissionAsync();
            PermissionList val = new PermissionList();
            val.countries = await db.country.listEnableAsync();
            val.countries = val.countries.Where(p => permission.countries.Contains(p.id)).ToList();
            val.urls = await db.url.listAsync();
            if (countrySelected != "" && countrySelected != "000000" && permission.countries.Count() > 1)
            {
                val.urls = val.urls.Where(p => permission.countries.Contains(p.country) && p.country == new MongoDB.Bson.ObjectId(countrySelected)).ToList();
            }
            else if (countrySelected == "" && permission.countries.Count() > 1)
            {
                val.urls = val.urls.Where(p => permission.countries.Contains(p.country) && p.country == permission.countries.FirstOrDefault()).ToList();
            }
            else
            {
                val.urls = val.urls.Where(p => permission.countries.Contains(p.country)).ToList();
            }
            return val;
        }

        // GET: /Url/
        [HttpGet]
        public async Task<IActionResult> Index(string countrySelect = "")
        {
            try
            {
                PermissionList obj = await LoadEnableByPermissionAsync(countrySelect);
                List<Url> list = obj.urls;
                ViewBag.countries = obj.countries;


                if (countrySelect == "")
                {
                    ViewBag.countrySelected = obj.countries[0].id.ToString();
                }
                else
                {
                    ViewBag.countrySelected = "";
                }
                ViewData["countriesData"] = getCountriesListWithDefult(obj, countrySelect);
                string dataJson = JsonConvert.SerializeObject(getListOfCountryUrl(obj));
                ViewBag.data = dataJson;

                await writeEventAsync(list.Count().ToString(), LogEvent.lis);
                return View(list);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }

        }

        // GET: /Url/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Url entity = await db.url.byIdAsync(id);
                ViewBag.country = await db.country.byIdAsync(entity.country.ToString());
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // GET: /Url/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await generateListCountriesAsync("");
            await generateListTypeAsync();
            return View();
        }


        // POST: /Url/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUrl()
        {
            try
            {
                var form = HttpContext.Request.Form;
                int count = form.Keys.Where(p => p.Contains("url_name_")).Count();
                List<UrlData> url_data = new List<UrlData>();
                string country_id = form["country"];
                for (int i = 1; i <= count; i++)
                {
                    url_data.Add(new UrlData()
                    {
                        name = form["url_name_" + i.ToString()],
                        value = form["url_value_" + i.ToString()],
                        forc_type = (ForecastType)int.Parse(form["forc_type_" + i.ToString()]),
                        prob_type = (CategoryUrl)int.Parse(form["prob_type_" + i.ToString()]),

                    });
                }
                if (url_data.Count() == 0)
                    return RedirectToAction("Create");
                Url entity = new Url()
                {
                    country = MongoDB.Bson.ObjectId.Parse(country_id),
                    type = (UrlTypes)int.Parse(form["type"]),
                    urls = url_data
                };
                await db.url.insertAsync(entity);
                await writeEventAsync(ModelState.ToString(), LogEvent.err);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Create");
            }
        }

        // GET: /Url/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Url entity = await db.url.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await generateListCountriesAsync(entity.country.ToString());
                await generateListTypeAsync();
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /Url/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUrl()
        {
            try
            {
                var form = HttpContext.Request.Form;
                Url current_entity = await db.url.byIdAsync(form["id"]);

                int count = form.Keys.Where(p => p.Contains("url_name_")).Count();
                List<UrlData> url_data = new List<UrlData>();
                string country_id = form["country"];
                for (int i = 1; i <= count; i++)
                {
                    url_data.Add(new UrlData()
                    {
                        name = form["url_name_" + i.ToString()],
                        value = form["url_value_" + i.ToString()],
                        forc_type = (ForecastType)int.Parse(form["forc_type_" + i.ToString()]),
                        prob_type = (CategoryUrl)int.Parse(form["prob_type_" + i.ToString()]),

                    });
                }
                if (url_data.Count() == 0)
                    return RedirectToAction("Edit", new RouteValueDictionary { { "id", current_entity.id } });
                Url entity = new Url()
                {
                    id = current_entity.id,
                    country = MongoDB.Bson.ObjectId.Parse(country_id),
                    type = (UrlTypes)int.Parse(form["type"]),
                    urls = url_data
                };
                await db.url.updateAsync(current_entity, entity);
                await writeEventAsync(entity.ToString(), LogEvent.upd);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var form = HttpContext.Request.Form;
                await generateListCountriesAsync(form["id"]);
                Url current_entity = await db.url.byIdAsync(form["id"]);
                await writeExceptionAsync(ex);
                return RedirectToAction("Edit", new RouteValueDictionary { { "id", current_entity.id } });
            }
        }

        // GET: /Url/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    await writeEventAsync("Search without id", LogEvent.err);
                    return new BadRequestResult();
                }
                Url entity = await db.url.byIdAsync(id);
                if (entity == null)
                {
                    await writeEventAsync("Not found id: " + id, LogEvent.err);
                    return new NotFoundResult();
                }
                await writeEventAsync("Search id: " + id, LogEvent.rea);
                return View(entity);
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return View();
            }
        }

        // POST: /Url/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Url entity = await db.url.byIdAsync(id);
                await db.url.deleteAsync(entity);
                await writeEventAsync(entity.ToString(), LogEvent.del);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await writeExceptionAsync(ex);
                return RedirectToAction("Delete", new { id = id });
            }
        }


        private async Task<bool> generateListCountriesAsync(string selected)
        {
            var obj = await LoadEnableByPermissionAsync();
            // Filter states by permission by countries            
            var countries = obj.countries.Select(p => new { id = p.id.ToString(), name = p.name });
            if (string.IsNullOrEmpty(selected))
                ViewData["country"] = new SelectList(countries, "id", "name");
            else
                ViewData["country"] = new SelectList(countries, "id", "name", selected);
            return countries.Count() > 0;
        }


        private async Task generateListTypeAsync()
        {
            // List climate variables
            var types = from UrlTypes q in Enum.GetValues(typeof(UrlTypes))
                           select new { id = (int)q, name = q.ToString() };
            ViewBag.type = new SelectList(types, "id", "name");

            var forc_type = from ForecastType q in Enum.GetValues(typeof(ForecastType))
                        select new { id = (int)q, name = q.ToString() };
            ViewBag.forc_type = new SelectList(forc_type, "id", "name");

            var prob_type = from CategoryUrl q in Enum.GetValues(typeof(CategoryUrl))
                        select new { id = (int)q, name = q.ToString() };
            ViewBag.prob_type = new SelectList(prob_type, "id", "name");
        }



        private List<Object> getListOfCountryUrl(PermissionList obj)
        {
            List<object> listCountryData = new List<object>();
            foreach (Country country in obj.countries)
            {
                IEnumerable<Url> rc = obj.urls.Where(s => s.country == country.id);

                var countryData = new
                {
                    countryId = country.id,
                    listData = rc
                };
                listCountryData.Add(countryData);
            }
            return listCountryData;
        }

        private SelectList getCountriesListWithDefult(PermissionList obj, string selectedCountryId)
        {

            List<SelectListItem> originalList = new List<SelectListItem>(obj.countries.Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.name }));

            originalList.Insert(0, new SelectListItem { Value = "000000", Text = "------", Selected = false });

            if (string.IsNullOrEmpty(selectedCountryId) && originalList.Count > 0)
            {
                SelectListItem selectedItem = originalList[1];
                selectedItem.Selected = true;
            }
            else
            {
                SelectListItem selectedItem = originalList.FirstOrDefault(item => item.Value == selectedCountryId);
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;
                }
            }
            SelectList selectList = new SelectList(originalList, "Value", "Text");


            return selectList;
        }

        [HttpGet]
        public async Task<ActionResult> ObtainCountryFilterData()
        {
            var obj = await LoadEnableByPermissionAsync();
            string dataJson = JsonConvert.SerializeObject(getListOfCountryUrl(obj));
            return Content(dataJson, "application/json");
        }


        [HttpGet]
        public async Task<ActionResult> GetSelectData()
        {
            var forc_type = from ForecastType q in Enum.GetValues(typeof(ForecastType))
                            select new { id = (int)q, name = q.ToString() };

            var prob_type = from CategoryUrl q in Enum.GetValues(typeof(CategoryUrl))
                            select new { id = (int)q, name = q.ToString() };
            var jsonData = new
            {
                forc_type,
                prob_type,
            };
            string dataJson = JsonConvert.SerializeObject(jsonData);
            return Content(dataJson, "application/json");
        }
    }
}
