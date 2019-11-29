using Newtonsoft.Json;

namespace GeoQuizCore.Models
{
    public class Question
    {
        [JsonProperty]
        public string QuestionString { get; private set; }
        [JsonProperty]
        public string[] Choices { get; private set; }

        public Question(string question, string[] choices)
        {
            QuestionString = question;
            Choices = choices;
        }
    }
}