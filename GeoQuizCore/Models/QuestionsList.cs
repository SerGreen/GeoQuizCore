using GeoQuizCore.Database.DatabaseClasses;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeoQuizCore.Models
{
    public class QuestionsList
    {
        [JsonProperty]
        private List<Country> countries;

        [JsonProperty]
        public int CorrectAnswersCount { get; private set; } = 0;
        [JsonIgnore]
        public float CorrectAnswersPercent { get { return (float)CorrectAnswersCount / Count; } }

        [JsonProperty]
        public int WrongAnswersCount { get; private set; } = 0;
        [JsonIgnore]
        public float WrongAnswersPercent { get { return (float)WrongAnswersCount / Count; } }

        [JsonProperty]
        public int LongestCorrectStreak { get; private set; } = 0;
        [JsonProperty]
        public int CurrentCorrectStreak { get; private set; } = 0;

        [JsonProperty]
        public int LongestWrongStreak { get; private set; } = 0;
        [JsonProperty]
        public int CurrentWrongStreak { get; private set; } = 0;

        [JsonProperty]
        public int CurrentQuestionIndex { get; private set; } = 0;

        [JsonProperty]
        public float PointsMultiplier { get; private set; } = 1.0f;
        private readonly float maxPointsMultiplier = 10.0f;
        private readonly int basicPointsForAnswer = 100;
        public int PointsForAnswer => (int)(basicPointsForAnswer * PointsMultiplier);
        public int Score { get; set; } = 0;

        //public QuestionAnswerPair this[int index]
        //{ get { return index < Count ? questions[index] : null; } }

        [JsonIgnore]
        public int Count => countries.Count;

        [JsonIgnore]
        public Question CurrentQuestion => CurrentQuestionAnswerPair?.Question;

        [JsonProperty]
        private QuestionAnswerPair CurrentQuestionAnswerPair { get; set; }

        [JsonIgnore]
        public string CorrectAnswer => CurrentQuestionAnswerPair?.Answer;

        [JsonIgnore]
        public bool EndReached { get { return CurrentQuestionIndex >= Count; } }

        /// <summary>
        /// This will create empty list of questions. You should not use this constructor.
        /// [future me]: then why is it public? :thonk:
        /// [a few hours later]: because without it serializer/deserializer shoots itself in the foot
        /// </summary>
        public QuestionsList() : this(new List<Country>(), null)
        { }

        /// <summary>
        /// This constructor should be used to create a list.
        /// </summary>
        public QuestionsList(List<Country> countries, QuestionBuilder builder)
        {
            this.countries = countries;
            this.CurrentQuestionAnswerPair = builder?.GetNextQuestion(countries[CurrentQuestionIndex]);
            CurrentQuestionIndex = 0;
            CorrectAnswersCount = 0;
            WrongAnswersCount = 0;
        }

        public bool TestAnswer(string answer, bool updateStats = true)
        {
            bool result = CurrentQuestionAnswerPair?.TestAnswer(answer) ?? false;
            if (!EndReached && updateStats)
            {
                if (result)
                {
                    CorrectAnswersCount++;
                    CurrentCorrectStreak++;
                    if (CurrentCorrectStreak > LongestCorrectStreak)
                        LongestCorrectStreak = CurrentCorrectStreak;
                    CurrentWrongStreak = 0;

                    // Add points
                    Score += PointsForAnswer;
                    if (CurrentCorrectStreak > 1 && PointsMultiplier < maxPointsMultiplier)
                        PointsMultiplier += 0.1f;
                }
                else
                {
                    WrongAnswersCount++;
                    CurrentWrongStreak++;
                    if (CurrentWrongStreak > LongestWrongStreak)
                        LongestWrongStreak = CurrentWrongStreak;
                    CurrentCorrectStreak = 0;

                    PointsMultiplier = 1.0f;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns True if question was successfully switched; False if the end is reached
        /// </summary>
        public bool AdvanceToNextQuestion(QuestionBuilder questionBuilder)
        {
            CurrentQuestionIndex++;
            if (EndReached)
                return false;

            CurrentQuestionAnswerPair = questionBuilder.GetNextQuestion(countries[CurrentQuestionIndex]);
            return true;
        }
    }
}