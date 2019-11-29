using GeoQuizCore.Database.DatabaseClasses;
using GeoQuizCore.Infrastructure;
using GeoQuizCore.Models;
using GeoQuizCore.Models.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GeoQuizCore.Controllers
{
    public class MenuController : Controller
    {
        private readonly GeoDBDataContext db;

        public MenuController(GeoDBDataContext context)
        {
            db = context;
        }

        public static string Nameof => nameof(MenuController).Replace("Controller", "");

        // GET: Menu
        [HttpGet]
        public ActionResult Index(GameSettings settings, string lang)
        {
            HttpContext.Session.SetString("Language", lang);

            var viewModel = new MenuIndexViewModel()
            {
                Settings = settings,
                ModalViewModel = new MenuIndexModalViewModel()
                {
                    AllowedNonSovereignIds = settings.AllowedNonSovereignIds,
                    AllCountries = db.Countries.Where(x => !x.IsSovereign)
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ActionName(nameof(Index))]
        public ActionResult IndexPost(GameSettings settings, GameMode gameMode = GameMode.FlagByCountry)
        {
            settings.GameMode = gameMode;
            return RedirectToAction(nameof(QuizController.Index), QuizController.Nameof);
        }

        [HttpPost]
        public void SaveGameSettings(GameSettingsSave settings)
        {
            // GameSettingsSave is built by deafult model binder from data provided by ajax request, instead of retrieving from Session by custom GameSettings model binder
            // GameSettingsSave is identical to GameSettings class otherwise
            HttpContext.Session.SetObjectAsJson("Settings", settings);
        }
    }
}