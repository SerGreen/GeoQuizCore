using GeoQuizCore.Database.DatabaseClasses;
using GeoQuizCore.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GeoQuizCore.Models
{
    public class FlagByCountryQuestionBuilder : QuestionBuilder
    {
        public FlagByCountryQuestionBuilder(GameSettings settings, GeoDBDataContext db, string language)
            : base(settings, db, language)
        { }

        public override QuestionAnswerPair GetNextQuestion(Country country)
        {
            int calculatedDistractorsAmount = Math.Max((int)settings.Difficulty, settings.DistractorsAmount);

            // Get localized name or default one if localization is missing
            string question = country.Localizations.Where(l => l.Language == language).Select(l => l.Name).FirstOrDefault() ?? country.Name;
            string answer = country.Id.ToString();
            // Select distractors: 
            // 1. Select all similar flags and order by similarity
            // 2. Take N most similar and randomize (the greater difficulty, the smaller the N value is => the more similar distractors are selected)
            // 3. Select M entries, where M is a number of distraction options
            string[] distractors = db.FlagNeighbours
                .AsNoTracking()
                .Include(fn => fn.Country2)
                .Where(fn => fn.CountryId1 == country.Id &&
                             settings.Continents.Contains(fn.Country2.Continent) &&
                            (fn.Country2.IsSovereign || settings.AllowedNonSovereignIds.Contains(fn.CountryId2)))
                .OrderBy(x => x.Distance)
                .Take(calculatedDistractorsAmount)
                .Shuffle()
                .Take(settings.DistractorsAmount)
                .Select(x => x.CountryId2.ToString())
                .ToArray();

            return new QuestionAnswerPair(question, answer, distractors);
        }
    }
}
