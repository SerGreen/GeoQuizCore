using GeoQuizCore.Database.DatabaseClasses;
using System.Collections.Generic;

namespace GeoQuizCore.Models
{
    public class MenuIndexModalViewModel
    {
        public List<int> AllowedNonSovereignIds { get; set; }
        public IEnumerable<Country> AllCountries { get; set; }
    }
}