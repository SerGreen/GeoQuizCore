using GeoQuizCore.Database.DatabaseClasses;
using GeoQuizCore.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GeoQuizCore.Models
{
    public class CountryByFlagQuestionBuilder : QuestionBuilder
    {
        public CountryByFlagQuestionBuilder(GameSettings settings, GeoDBDataContext db, string language)
            : base(settings, db, language)
        { }

        public override QuestionAnswerPair GetNextQuestion(Country country)
        {
            int calculatedDistractorsAmount = Math.Max((int)settings.Difficulty, settings.DistractorsAmount);

            string question = country.Id.ToString();
            Localization localized = country.Localizations.Where(l => l.Language == language).FirstOrDefault();
            string answer = localized?.Name ?? country.Name;
            string alias = localized?.AliasName;
            // Everything as in flags mode, but now question is country ID (flag image selected by id) and answer and distractors are country names
            string[] distractors = db.FlagNeighbours
                .AsNoTracking()
                .Include(fn => fn.Country2)
                .ThenInclude(c2 => c2.Localizations)
                .Where(fn => fn.CountryId1 == country.Id &&
                             settings.Continents.Contains(fn.Country2.Continent) &&
                            (fn.Country2.IsSovereign || settings.AllowedNonSovereignIds.Contains(fn.CountryId2)))
                .OrderBy(x => x.Distance)
                .Take(calculatedDistractorsAmount)
                .Shuffle()
                .Take(settings.DistractorsAmount)
                .Select(c2 => c2.Country2.Localizations.Where(l => l.Language == language).Select(l => l.Name).FirstOrDefault() ?? c2.Country2.Name)
                .ToArray();

            return new QuestionAnswerPair(question, answer, distractors, alias);
        }
    }
}
