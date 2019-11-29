using GeoQuizCore.Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeoQuizCore.Models
{
    public class QuestionAnswerPair
    {
        [JsonProperty]
        public Question Question { get; private set; }
        [JsonProperty]
        private int CorrectAnswerIndex { get; set; }
        [JsonIgnore]
        public string Answer { get { return Question.Choices[CorrectAnswerIndex]; } }
        [JsonProperty]
        public string AnswerAlias { get; private set; }

        public QuestionAnswerPair()
        { }

        public QuestionAnswerPair(string question, string answer, string[] distractors, string answerAlias = null)
        {
            List<string> choices = new List<string>(distractors);
            choices.Add(answer);
            choices = choices.Shuffle().ToList();

            CorrectAnswerIndex = choices.IndexOf(answer);
            AnswerAlias = answerAlias != null ? Regex.Replace(answerAlias, @"[\[|\]|'|’]", " ") : null;
            Question = new Question(question, choices.ToArray());
        }

        public bool TestAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return false;

            string playerAnswer = Regex.Replace(answer, @"[\[|\]|'|’]", " ");
            string correctAnswer = Regex.Replace(Answer, @"[\[|\]|'|’]", " ");
            return playerAnswer.Equals(correctAnswer, StringComparison.InvariantCultureIgnoreCase)
                || (AnswerAlias != null && playerAnswer.Equals(AnswerAlias, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}