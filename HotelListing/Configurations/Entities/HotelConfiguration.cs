using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Configurations.Entities
{
  public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
  {
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
      builder.HasData(
        new Hotel { Id = 1, Name = "Sandals Resort and SPA", Address = "way 4", CountryId = 1, Rating = 4.5 },
        new Hotel { Id = 2, Name = "Grand Paladium", Address = "way 3", CountryId = 2, Rating = 4.5 },
        new Hotel { Id = 3, Name = "Scandic", Address = "way 2", CountryId = 3, Rating = 4.5 },
        new Hotel { Id = 4, Name = "Hero", Address = "way 1", CountryId = 4, Rating = 4.5 }
      );
    }
  }
}
