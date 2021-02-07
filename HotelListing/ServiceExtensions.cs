using HotelListing.Data;
using HotelListing.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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

    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
      app.UseExceptionHandler(error => {
        error.Run(async context => {
          context.Response.StatusCode = StatusCodes.Status500InternalServerError;
          context.Response.ContentType = "application/json";
          var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

          if (contextFeature != null)
          {
            Log.Error($"Something went wrong in the {contextFeature.Error}.");
            await context.Response.WriteAsync(new Error 
            { 
              StatusCode = 500, 
              Message = "Internal Server Error. Please try again later."
            }.ToString());
          }
        });
      });
    }

    public static void ConfigureVersioning(this IServiceCollection services)
    {
      services.AddApiVersioning(options => {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = new HeaderApiVersionReader("api-version");
      });
    }

    public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
    {
      services.AddResponseCaching();
      services.AddHttpCacheHeaders((expirationOptions) => {
        expirationOptions.MaxAge = 65;
        expirationOptions.CacheLocation = CacheLocation.Private;
      }, 
      (validationOptions) => {
        validationOptions.MustRevalidate = true;      
      });
    }
  }
}
