using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using IdentityModel;

using NJsonSchema;
using NSwag.AspNetCore;
using System.Reflection;

using Latsic.IdApi1.Configuration;
using Latsic.IdApi1.AuthPolicies;

namespace IdApi1
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

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      // Register the Swagger services
      services.AddSwagger();

      services.AddAuthorization(options =>
      {
          options.AddPolicy("UserNumberAtLeast20", policy => policy.Requirements.Add(new MinUserNumberRequirement(20)));
          options.AddPolicy("AgeAtLeast16", policy => policy.Requirements.Add(new MinAgeRequirement(16)));
          options.AddPolicy("AgeAtLeast18", policy => policy.Requirements.Add(new MinAgeRequirement(18)));
          options.AddPolicy("AgeAtLeast21", policy => policy.Requirements.Add(new MinAgeRequirement(21)));
          options.AddPolicy("Admin", policy => policy.RequireClaim(JwtClaimTypes.Role, new string[] {"admin", "Admin", "ADMIN"}));
      });
      services.AddSingleton<IAuthorizationHandler, MinUserNumberHandler>();
      services.AddSingleton<IAuthorizationHandler, MinAgeHandler>();

      var apiSettingsSection = Configuration.GetSection("ApiSettings");
      var apiSettings = apiSettingsSection.Get<ApiSettings>();
      services.Configure<ApiSettings>(apiSettingsSection);

      services.AddAuthentication("Bearer")
      .AddIdentityServerAuthentication(options =>
      {
        options.Authority = apiSettings.AuthAuthorityUrl;
        options.ApiName = apiSettings.ApiName;
        options.RequireHttpsMetadata = false;
      });

      services.AddCors(options => {
        options.AddPolicy("default", policy => {
          policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
        });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }
      app.UseCors("default");
      app.UseAuthentication();
      //app.UseHttpsRedirection();
      // app.UseMvc();

      // Register the Swagger generator and the Swagger UI middlewares
      // Add Swagger UI with multiple documents
      app.UseSwaggerUi3WithApiExplorer(settings =>
      {
        settings.GeneratorSettings.DefaultPropertyNameHandling = 
          PropertyNameHandling.CamelCase;
        
        settings.GeneratorSettings.Title = "Authorization Test Endpoints";
        settings.GeneratorSettings.Description = "An API to test authorization with Identity Server 4";

      });



      app.UseMvc(routes =>
      {
        routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
