using GeoQuizCore.Models.Shared;

namespace GeoQuizCore.Models
{
    public class QuestionViewModel
    {
        public int Index { get; set; }
        public int TotalQuestionsCount { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int CorrectStreak { get; set; }
        public int PointsReward { get; set; }
        public int Score { get; set; }
        public Question Question { get; set; }
        public GameMode GameMode { get; set; }
        public int TimeLimit { get; set; }
    }
}