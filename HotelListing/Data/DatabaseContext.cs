using HotelListing.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
  public class DatabaseContext : IdentityDbContext<ApiUser>
  {
    public DatabaseContext(DbContextOptions options) : base(options) {}

    public DbSet<Country> Contries { get; set; }

    public DbSet<Hotel> Hotels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfiguration(new CountryConfiguration());
      modelBuilder.ApplyConfiguration(new HotelConfiguration());
      modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }
  }
}
