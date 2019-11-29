using GeoQuizCore.Database.DatabaseClasses;
using GeoQuizCore.Infrastructure;
using GeoQuizCore.Models;
using GeoQuizCore.Models.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoQuizCore.Controllers
{
    public class QuizController : Controller
    {
        private readonly GeoDBDataContext db;

        public QuizController(GeoDBDataContext context)
        {
            db = context;
        }

        public static string Nameof => nameof(QuizController).Replace("Controller", "");

        [HttpGet]
        public ActionResult Index(GameSettings settings)
        {
            List<Country> countries = GetSelectedCountries(settings);
            string language = HttpContext.Session.GetString("Language")?.ToUpper();
            QuestionBuilder questionBuilder;

            switch (settings.GameMode)
            {
                case GameMode.FlagByCountry:
                    questionBuilder = new FlagByCountryQuestionBuilder(settings, db, language); break;
                case GameMode.CountryByFlag:
                    questionBuilder = new CountryByFlagQuestionBuilder(settings, db, language); break;
                case GameMode.CapitalByCountry:
                    questionBuilder = new CapitalByCountryQuestionBuilder(settings, db, language); break;
                default:
                    return RedirectToAction(nameof(MenuController.Index), MenuController.Nameof);
            }

            QuestionsList questionsList = new QuestionsList(countries, questionBuilder);

            HttpContext.Session.SetObjectAsJson("Questions", questionsList);
            return View(nameof(Quiz), GetQuestionViewModel(questionsList));
        }

        private List<Country> GetSelectedCountries(GameSettings settings)
        {
            // select country if it is on allowed continent and if it is either sovereign or one of the allowed non-sovereigns
            return db.Countries
                .AsNoTracking()
                .Include(c => c.Localizations)
                .Where(c => settings.Continents.Contains(c.Continent) && (c.IsSovereign || (settings.AllowedNonSovereignIds.Contains(c.Id))))
                .Shuffle()
                .ToList();
        }

        private QuestionViewModel GetQuestionViewModel(QuestionsList questions)
        {
            GameSettings settings = HttpContext.Session.GetObjectFromJson<GameSettings>("Settings");
            return new QuestionViewModel()
            {
                Index = questions.CurrentQuestionIndex,
                TotalQuestionsCount = questions.Count,
                CorrectAnswers = questions.CorrectAnswersCount,
                WrongAnswers = questions.WrongAnswersCount,
                Question = questions.CurrentQuestion,
                GameMode = settings.GameMode,
                TimeLimit = settings.TimeLimit,
                CorrectStreak = questions.CurrentCorrectStreak,
                PointsReward = questions.PointsForAnswer,
                Score = questions.Score
            };
        }

        [HttpPost]
        public ActionResult Quiz(QuestionsList questions, GameSettings settings, string answer, int questionIndex, string lang)
        {
            // Check answer given by player and go to next question
            if (questions.TestAnswer(answer) == false)
            {
                // If answer is wrong => set mistake message
                // No answer = timeout
                if (answer is null)
                    TempData["Mistake"] = "Timeout";
                // Answer was wrong
                else
                {
                    string mistakeMessage = null;

                    switch (settings.GameMode)
                    {
                        case GameMode.FlagByCountry:
                            var c = db.Countries.FirstOrDefault(x => x.Id == int.Parse(answer));
                            mistakeMessage = c.Localizations.Where(x => x.Language == lang.ToUpper()).FirstOrDefault()?.Name ?? c.Name; break;
                        case GameMode.CountryByFlag:
                        case GameMode.CapitalByCountry:
                            mistakeMessage = questions.CorrectAnswer; break;
                    }
                    TempData["Mistake"] = mistakeMessage;
                }
            }

            // Advance to the next question and save updated questions list to the session state
            string language = HttpContext.Session.GetString("Language")?.ToUpper();
            QuestionBuilder questionBuilder;
            switch (settings.GameMode)
            {
                case GameMode.FlagByCountry:
                    questionBuilder = new FlagByCountryQuestionBuilder(settings, db, language); break;
                case GameMode.CountryByFlag:
                    questionBuilder = new CountryByFlagQuestionBuilder(settings, db, language); break;
                case GameMode.CapitalByCountry:
                    questionBuilder = new CapitalByCountryQuestionBuilder(settings, db, language); break;
                default:
                    throw new Exception($"What is this \"{settings.GameMode}\" game mode?");
            }
            questions.AdvanceToNextQuestion(questionBuilder);
            HttpContext.Session.SetObjectAsJson("Questions", questions);

            // Return view
            // If this is not Ajax request => return whole page
            bool isAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (!isAjaxRequest)
                return View(GetQuestionViewModel(questions));
            // If Ajax => return only partial view with next question
            else
            {
                string partialViewName = null;
                switch (settings.GameMode)
                {
                    case GameMode.FlagByCountry:
                        partialViewName = "PartialFlagByCountry"; break;
                    case GameMode.CountryByFlag:
                        partialViewName = "PartialCountryByFlag"; break;
                    case GameMode.CapitalByCountry:
                        partialViewName = "PartialCapitalByCountry"; break;
                }

                return PartialView(partialViewName, GetQuestionViewModel(questions));
            }
        }

        [HttpGet]
        public ActionResult Results(QuestionsList questions, GameSettings settings)
        {
            return View(new ResultsViewModel() { Questions = questions, GameSettings = settings });
        }
    }
}