using HotelListing.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using AutoMapper;
using HotelListing.Configurations;
using HotelListing.IRepository;
using HotelListing.Repository;
using Microsoft.AspNetCore.Identity;
using HotelListing.Services;
using AspNetCoreRateLimit;

namespace HotelListing
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<DatabaseContext>(options => 
        options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
      );

      // Used for rate limit
      services.AddMemoryCache();
      services.ConfigureRateLimiting();
      services.AddHttpContextAccessor();

      services.ConfigureHttpCacheHeaders();

      services.AddAuthentication();
      services.ConfigureIdentity();
      services.ConfigureJwt(Configuration);

      // Transient, Always create a new IUnitOfWork
      services.AddTransient<IUnitOfWork, UnitOfWork>();
      services.AddScoped<IAuthenticationManager, AuthenticationManager>();

      services.ConfigureCors();

      services.AddAutoMapper(typeof(MapperInitializer));

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
      });

      services.AddControllers(config => {
        // configures cache profile
        config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { 
          Duration = 120
        });
      }).AddNewtonsoftJson(option => 
      option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
      );

      services.ConfigureVersioning();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger();
      //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));
      app.UseSwaggerUI(c => 
      {
        // Fixed path for production mode IIS
        string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
        c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Hotel Listing API"); 
      });

      app.ConfigureExceptionHandler();

      app.UseHttpsRedirection();

      app.UseCors("CorsPolicyAllowAll");

      app.UseResponseCaching();
      app.UseHttpCacheHeaders();
      app.UseIpRateLimiting();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
