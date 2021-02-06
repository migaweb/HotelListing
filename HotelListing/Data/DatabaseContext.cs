using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
  public class DatabaseContext : DbContext
  {
    public DatabaseContext(DbContextOptions options) : base(options) {}

    public DbSet<Country> Contries { get; set; }

    public DbSet<Hotel> Hotels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Country>().HasData(
        new Country { Id = 1, Name = "Sweden", ShortName = "SE" },
        new Country { Id = 2, Name = "Jamaica", ShortName = "JM" },
        new Country { Id = 3, Name = "Bahamas", ShortName = "BS" },
        new Country { Id = 4, Name = "Cayman Island", ShortName = "CI" }
        );
      modelBuilder.Entity<Hotel>().HasData(
        new Hotel { Id = 1, Name = "Sandals Resort and SPA", Address = "way 4", CountryId = 1, Rating = 4.5 },
        new Hotel { Id = 2, Name = "Grand Paladium", Address = "way 3", CountryId = 2, Rating = 4.5 },
        new Hotel { Id = 3, Name = "Scandic", Address = "way 2", CountryId = 3, Rating = 4.5 },
        new Hotel { Id = 4, Name = "Hero", Address = "way 1", CountryId = 4, Rating = 4.5 }
        );
      base.OnModelCreating(modelBuilder);
    }
  }
}
