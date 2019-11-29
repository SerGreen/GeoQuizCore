using GeoQuizCore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GeoQuizCore.Infrastructure
{
    public class BinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(GameSettings))
                return new GameSettingsModelBinder();
            else if (context.Metadata.ModelType == typeof(QuestionsList))
                return new QuestionsModelBinder();

            return null;
        }
    }
}
