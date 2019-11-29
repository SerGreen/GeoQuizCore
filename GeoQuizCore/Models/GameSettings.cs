using GeoQuizCore.Models.Shared;
using System.Collections.Generic;

namespace GeoQuizCore.Models
{
    public class GameSettings
    {
        public GameMode GameMode { get; set; } = GameMode.FlagByCountry;
        public Difficulty Difficulty { get; set; } = Difficulty.Medium;
        public int DistractorsAmount { get; set; } = 3;
        public int TimeLimit { get; set; } = 10;
        public List<int> AllowedNonSovereignIds { get; set; } = new List<int>();
        public List<string> Continents { get; set; } = new List<string>() { "NA", "SA", "EU", "AS", "AF", "AU" };
    }
}