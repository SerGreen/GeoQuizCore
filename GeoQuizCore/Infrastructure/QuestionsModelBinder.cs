using GeoQuizCore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace GeoQuizCore.Infrastructure
{
    public class QuestionsModelBinder : IModelBinder
    {
        private const string sessionKey = "Questions";

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            QuestionsList questions = null;
            if (bindingContext.HttpContext.Session != null)
            {
                questions = bindingContext.HttpContext.Session.GetObjectFromJson<QuestionsList>(sessionKey);
            }

            if (questions == null)
            {
                questions = new QuestionsList();
                if (bindingContext.HttpContext.Session != null)
                {
                    bindingContext.HttpContext.Session.SetObjectAsJson(sessionKey, questions);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(questions);
            return Task.CompletedTask;
        }
    }
}