using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoQuizCore.Database.DatabaseClasses
{
    public class GeoDBDataContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<FlagNeighbour> FlagNeighbours { get; set; }
        public DbSet<Localization> Localizations { get; set; }

        public GeoDBDataContext(DbContextOptions<GeoDBDataContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Localization>()
                .HasKey(loc => new { loc.Language, loc.CountryId });

            modelBuilder.Entity<Localization>()
                .HasOne(l => l.Country)
                .WithMany(c => c.Localizations)
                .HasForeignKey(l => l.CountryId);

            modelBuilder.Entity<FlagNeighbour>()
                .HasKey(fn => fn.Id);

            modelBuilder.Entity<FlagNeighbour>()
                .HasOne(fn => fn.Country1)
                .WithMany(c => c.FlagNeighbours1)
                .HasForeignKey(fn => fn.CountryId1);

            modelBuilder.Entity<FlagNeighbour>()
                .HasOne(fn => fn.Country2)
                .WithMany(c => c.FlagNeighbours2)
                .HasForeignKey(fn => fn.CountryId2);
        }
    }

    [Table("Countries")]
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Capital { get; set; }
        [JsonIgnore]
        public string Continent { get; set; }
        [JsonIgnore]
        public bool IsSovereign { get; set; }

        [InverseProperty("Country1")]
        [JsonIgnore]
        public virtual ICollection<FlagNeighbour> FlagNeighbours1 { get; set; } = new List<FlagNeighbour>();
        [InverseProperty("Country2")]
        [JsonIgnore]
        public virtual ICollection<FlagNeighbour> FlagNeighbours2 { get; set; } = new List<FlagNeighbour>();
        [InverseProperty("Country")]
        public virtual ICollection<Localization> Localizations { get; set; } = new List<Localization>();
    }

    [Table("FlagNeighbours")]
    public class FlagNeighbour
    {
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int CountryId1 { get; set; }
        [ForeignKey("Id")]
        public int CountryId2 { get; set; }
        public double Distance { get; set; }

        public virtual Country Country1 { get; set; }
        public virtual Country Country2 { get; set; }
    }

    [Table("Localization")]
    public class Localization
    {
        public string Language { get; set; }
        [ForeignKey("Id")]
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Capital { get; set; }
        public string AliasName { get; set; }

        [JsonIgnore]
        public virtual Country Country { get; set; }
    }
}
