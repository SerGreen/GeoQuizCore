using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeoQuizCore.Infrastructure
{
    public class RouteCultureProvider : IRequestCultureProvider
    {
        private CultureInfo defaultCulture;
        private CultureInfo defaultUICulture;

        public RouteCultureProvider(RequestCulture requestCulture)
        {
            defaultCulture = requestCulture.Culture;
            defaultUICulture = requestCulture.UICulture;
        }

        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            //Parsing language from url path, which looks like "/en/home/index"
            PathString url = httpContext.Request.Path;

            // Test any culture in route
            if (url.ToString().Length <= 1)
            {
                // Set default Culture and default UICulture
                return Task.FromResult<ProviderCultureResult>(new ProviderCultureResult(defaultCulture.TwoLetterISOLanguageName, defaultUICulture.TwoLetterISOLanguageName));
            }

            var parts = httpContext.Request.Path.Value.Split('/');
            var lang = parts[1];

            // Test if the culture is properly formatted
            if (!Regex.IsMatch(lang, @"^[a-z]{2}(-[A-Z]{2})?$"))
            {
                // Set default Culture and default UICulture
                return Task.FromResult<ProviderCultureResult>(new ProviderCultureResult(defaultCulture.TwoLetterISOLanguageName, defaultUICulture.TwoLetterISOLanguageName));
            }
            // Set Culture and UICulture from route culture parameter
            return Task.FromResult<ProviderCultureResult>(new ProviderCultureResult(lang));
        }
    }
}
