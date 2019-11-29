using GeoQuizCore.Models.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoQuizCore.Infrastructure
{
    public static class Helpers
    {
        private static readonly string[] continents = { "NA", "SA", "EU", "AS", "AF", "AU" };

        public static string GetContinentsList(this IHtmlHelper helper, List<string> shorts)
        {
            IServiceProvider services = helper.ViewContext.HttpContext.RequestServices;
            IStringLocalizer<SharedResources> Localization = services.GetRequiredService<IStringLocalizer<SharedResources>>();

            List<string> list = new List<string>();
            bool[] isPresent = new bool[continents.Length];
            for (int i = 0; i < continents.Length; i++)
                if (shorts.Contains(continents[i]))
                    isPresent[i] = true;

            if (isPresent.All(x => x == true))
                return Localization["whole_world"];

            if (isPresent[0] && isPresent[1])
                list.Add(Localization["north_and_south_americas"]);
            else
            {
                if (isPresent[0])
                    list.Add(Localization["north_america"]);
                if (isPresent[1])
                    list.Add(Localization["south_america"]);
            }

            if (isPresent[2])
                list.Add(Localization["europe"]);
            if (isPresent[3])
                list.Add(Localization["asia"]);
            if (isPresent[4])
                list.Add(Localization["africa"]);
            if (isPresent[5])
                list.Add(Localization["australia"]);

            return string.Join(" | ", list);
        }

        public static string GetDificultyString(this IHtmlHelper helper, Difficulty difficulty)
        {
            IServiceProvider services = helper.ViewContext.HttpContext.RequestServices;
            IStringLocalizer<SharedResources> Localization = services.GetRequiredService<IStringLocalizer<SharedResources>>();

            switch (difficulty)
            {
                case Difficulty.Easy:
                    return Localization["difficulty_easy"];
                case Difficulty.Medium:
                    return Localization["difficulty_medium"];
                case Difficulty.Hard:
                    return Localization["difficulty_hard"];
                case Difficulty.VeryHard:
                    return Localization["difficulty_very_hard"];
                default:
                    return "OVER 9000";
            }
        }
    }
}