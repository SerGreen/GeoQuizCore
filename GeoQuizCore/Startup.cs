using GeoQuizCore.Controllers;
using GeoQuizCore.Database.DatabaseClasses;
using GeoQuizCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace GeoQuizCore
{
    public class Startup
    {
        private string _contentRootPath = "";

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _contentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Model binding
            services.AddMvc(config => config.ModelBinderProviders.Insert(0, new BinderProvider()))
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddControllersAsServices()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            //Added for session state
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            // Localization
            services.AddLocalization(o => o.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ru")
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new RouteCultureProvider(options.DefaultRequestCulture));
            });
            // Makes non-latin characters render properly instead of being html &#x0443;&#x044b;&#x0441;&#x0444;&#x0437;&#x0443;&#x0432;
            services.Configure<WebEncoderOptions>(options =>
            { options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All); });

            // Database
            string connectionString = Configuration.GetConnectionString("GeoDBConnectionString");
            if (connectionString.Contains("%CONTENTROOTPATH%"))
                connectionString = connectionString.Replace("%CONTENTROOTPATH%", _contentRootPath);
            services.AddDbContext<GeoDBDataContext>(options => options
                .UseLazyLoadingProxies()
                .UseSqlServer(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: null,
                   template: "",
                   defaults: new { controller = MenuController.Nameof, action = nameof(MenuController.Index), lang = "en" }
                );
                routes.MapRoute(
                   name: null,
                   template: "quiz",
                   defaults: new { controller = QuizController.Nameof, action = nameof(QuizController.Index), lang = "en" }
                );
                routes.MapRoute(
                   name: null,
                   template: "results",
                   defaults: new { controller = QuizController.Nameof, action = nameof(QuizController.Results), lang = "en" }
                );
                routes.MapRoute(
                   name: null,
                   template: "{lang}",
                   defaults: new { controller = MenuController.Nameof, action = nameof(MenuController.Index) }
                );
                routes.MapRoute(
                   name: null,
                   template: "{lang}/quiz",
                   defaults: new { controller = QuizController.Nameof, action = nameof(QuizController.Index) }
                );
                routes.MapRoute(
                   name: null,
                   template: "{lang}/results",
                   defaults: new { controller = QuizController.Nameof, action = nameof(QuizController.Results) }
                );
                routes.MapRoute(
                   name: "default",
                   template: "{lang}/{controller}/{action}"
                );
            });
        }
    }
}
