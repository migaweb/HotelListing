using HotelListing.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing
{
  public static class ServiceExtensions
  {
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
      var jwtSettings = configuration.GetSection("Jwt");
      var key = Environment.GetEnvironmentVariable("KEY");

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

      }).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings.GetSection("Issuer").Value,
          ValidateAudience = false,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
      });
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
      var builder = services.AddIdentityCore<ApiUser>(e =>
        e.User.RequireUniqueEmail = true
      );

      builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
      builder.AddEntityFrameworkStores<DatabaseContext>()
             .AddDefaultTokenProviders();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
      services.AddCors(corsOptions => {
        corsOptions.AddPolicy("CorsPolicyAllowAll", builder =>
          builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader());
      });
    }
  }
}
