using GeoQuizCore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace GeoQuizCore.Infrastructure
{
    public class GameSettingsModelBinder : IModelBinder
    {
        private const string sessionKey = "Settings";

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            GameSettings settings = null;

            // Try to retrieve from Session
            settings = bindingContext.HttpContext.Session?.GetObjectFromJson<GameSettings>(sessionKey);

            // If no success, then create new object and save it
            if (settings == null)
            {
                settings = new GameSettings();
                if (bindingContext.HttpContext.Session != null)
                    bindingContext.HttpContext.Session.SetObjectAsJson(sessionKey, settings);
            }

            bindingContext.Result = ModelBindingResult.Success(settings);
            return Task.CompletedTask;
        }
    }
}