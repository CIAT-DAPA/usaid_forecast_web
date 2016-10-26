using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIAT.DAPA.USAID.Forecast.Data.Database;
using System.Net;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using System;
using CIAT.DAPA.USAID.Forecast.Data.Factory;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    public class LocalityController : Controller
    {
        /// <summary>
        /// Get or set object to connect with database
        /// </summary>
        private ForecastDB db { get; set; }
        private LocalityFactory factory { get; set; }

        public LocalityController():base()
        {
            db = new ForecastDB("mongodb://localhost:27017");
            factory = db.locality;
        }

        // GET: /Locality/
        public async Task<IActionResult> Index()
        {
            var list = await factory.listEnableAsync();
            return View(list);
        }

        // GET: EPS/Details/5
        public async Task<IActionResult> Details(string id)
        {
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
            Locality entity = await factory.byIdAsync(id);
            if (entity == null)
            {
                //return HttpNotFound();
            }
            return View(entity);
        }

        // GET: /Locality/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: /Locality/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Locality entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await TryUpdateModelAsync<Locality>(entity);
                    entity.date_register = DateTime.Now;
                    entity.date_updated = entity.date_register;
                    entity.enable = true;                    
                    await factory.insertAsync(entity);
                    /*if (!ePS.isValid())
                        throw new Exception("La EPS no es valida");
                    db.guardar();*/
                    return RedirectToAction("Index");
                }

                return View(entity);
            }
            catch (Exception ex)
            {
                /*foreach (var issue in ePS.GetReglasValidacion())
                    ModelState.AddModelError(issue.propiedad, issue.mensaje);
                return View(ePS);*/
                return View(entity);
            }

        }

        // GET: /Locality/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            /*
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
            Locality entity = await factory.byIdAsync(id);
            /*if (entity == null)
            {
                return HttpNotFound();
            }*/
            return View(entity);
        }

        // POST: /Locality/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Locality entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await TryUpdateModelAsync<Locality>(entity);
                    entity.date_updated = DateTime.Now;
                    await factory.insertAsync(entity);
                    
                    /*if (!ePS.isValid())
                        throw new Exception("La EPS no es valida");*/                    
                    return RedirectToAction("Index");
                }
                return View(entity);
            }
            catch (Exception ex)
            {
                /*
                foreach (var issue in ePS.GetReglasValidacion())
                    ModelState.AddModelError(issue.propiedad, issue.mensaje);*/
                return View(entity);
            }

        }
    }
}
