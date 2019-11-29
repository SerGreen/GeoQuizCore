using GeoQuizCore.Database.DatabaseClasses;
using GeoQuizCore.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GeoQuizCore.Models
{
    public class CapitalByCountryQuestionBuilder : QuestionBuilder
    {
        public CapitalByCountryQuestionBuilder(GameSettings settings, GeoDBDataContext db, string language)
            : base(settings, db, language)
        { }

        public override QuestionAnswerPair GetNextQuestion(Country country)
        {
            Localization localized = country.Localizations.Where(l => l.Language == language).FirstOrDefault();
            string question = localized?.Name ?? country.Name;
            string answer = localized?.Capital ?? country.Capital;
            string alias = localized?.AliasName;
            // Everything as in flags mode, but now question is country ID (flag image selected by id) and answer and distractors are country names
            string[] distractors = db.Countries
                .AsNoTracking()
                .Include(c => c.Localizations)
                .Where(c => c.Id != country.Id && (c.IsSovereign || settings.AllowedNonSovereignIds.Contains(c.Id)) && settings.Continents.Contains(c.Continent))
                .Shuffle()
                .Take(settings.DistractorsAmount)
                .Select(c => c.Localizations.Where(l => l.Language == language).Select(l => l.Capital).FirstOrDefault() ?? c.Capital)
                .ToArray();

            return new QuestionAnswerPair(question, answer, distractors, alias);
        }
    }
}
