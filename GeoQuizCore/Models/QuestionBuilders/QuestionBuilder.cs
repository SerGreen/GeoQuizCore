using GeoQuizCore.Database.DatabaseClasses;

namespace GeoQuizCore.Models
{
    public abstract class QuestionBuilder
    {
        protected readonly string language;
        protected readonly GameSettings settings;
        protected readonly GeoDBDataContext db;

        public QuestionBuilder(GameSettings settings, GeoDBDataContext db, string language)
        {
            this.language = language;
            this.settings = settings;
            this.db = db;
        }

        public abstract QuestionAnswerPair GetNextQuestion(Country country);
    }
}
