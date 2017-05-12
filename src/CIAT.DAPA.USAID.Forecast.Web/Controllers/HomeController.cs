using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CIAT.DAPA.USAID.Forecast.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {   
            return RedirectToAction("Index", "Clima");
        }

        public IActionResult AcercaDe()
        {
            return View();
        }
    }
}
