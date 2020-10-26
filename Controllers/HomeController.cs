using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZeitexportWeb.Models;
using ZeitexportWeb.Manager;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.Intrinsics.X86;

namespace ZeitexportWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("woche/{projektnummer?}/{kw?}")]
        public IActionResult Woche(string projektnummer, int kw)
        {
            if (kw == 0)
            {
                kw = Zeiten.GetKalenderwoche(DateTime.Now.Date);
            }

            // Keine Projektnummer
            if (string.IsNullOrEmpty(projektnummer))
            {
                ModelState.AddModelError("Projektnummer", "Es wird eine Projektnummer benötigt.");
                return View("Index");
            }

            var zeiten = Zeiten.LoadByProjektnummer(projektnummer, _config.GetConnectionString("DataModelConnection"));

            // Keine Einträge im Projekt
            if (zeiten.Count == 0)
            {
                ModelState.AddModelError("Projektnummer", "In diesem Projekt gibt es keine Einträge.");
                return View("Index");
            } else
            {
                // Keine Einträge in Kalenderwoche
                var zeitenInKw = zeiten.FindAll(Item => kw == Zeiten.GetKalenderwoche(Item.Datum.Value));

                if (zeitenInKw.Count == 0)
                {
                    ModelState.AddModelError("Datum", "In der angegebenen Kalenderwoche gibt es in dem Projekt keine Einträge.");
                    return View("Index");
                }
            }

            if (ModelState.IsValid)
            {
                LinkRepository.AddLink(projektnummer);

                var zeitenGruppiert = zeiten
                    .FindAll(Item => kw == Zeiten.GetKalenderwoche(Item.Datum.Value))
                    // Alle Einträge in der gegenemem Kalenderwoche holen
                    .OrderBy(Item => Item.Datum) // Nach Datum sortieren
                    .GroupBy(Item => Item.Datum); // Alle Einträge mit dem gleichen Datum gruppieren

                return View(zeitenGruppiert);
            } else
            {
                return View("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
