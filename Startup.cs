using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityModel;

using IdentityServer4.AccessTokenValidation;

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
        options.AddPolicy("ApiAccess", policy => policy.RequireClaim("ApiAccess", "IdApi1"));
      });
      services.AddSingleton<IAuthorizationHandler, MinUserNumberHandler>();
      services.AddSingleton<IAuthorizationHandler, MinAgeHandler>();

      var deployEnv = Configuration.GetSection("DeployEnv");
      services.Configure<DeployEnv>(deployEnv);

      var apiSettingsSection = Configuration.GetSection("ApiSettings");
      var apiSettings = apiSettingsSection.Get<ApiSettings>();
      services.Configure<ApiSettings>(apiSettingsSection);

      var tokenValidationConfig = new TokenValidationConfig(); 
      services.AddSingleton<ITokenValidationConfig>(tokenValidationConfig);

      services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
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

      // Store the tokenvalidation parameters so the can be changed during runtime.
      services.PostConfigure<JwtBearerOptions>(
        IdentityServerAuthenticationDefaults.AuthenticationScheme +
        "IdentityServerAuthenticationJwt", // IdentityServerAuthenticationDefaults.JwtAuthenticationScheme
        options =>
        {
          tokenValidationConfig.TokenValidationParameters = options.TokenValidationParameters;
        }
      );
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(
      IApplicationBuilder app,
      IHostingEnvironment env,
      IOptions<ApiSettings> apiSettings,
      IOptions<DeployEnv> deployEnv)
    {
      if(env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else if(!deployEnv.Value.ReverseProxy && env.IsProduction())
      {
        app.UseHsts();
      }

      if(!string.IsNullOrWhiteSpace(deployEnv.Value.BasePath))
      {
        app.Use(async (ContextBoundObject, next) =>
        {
          ContextBoundObject.Request.PathBase = deployEnv.Value.BasePath;
          await next.Invoke();
        });
      }

      if(deployEnv.Value.ReverseProxy)
      {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
          ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
      }

      app.UseCors("default");
      app.UseAuthentication();
      if(!deployEnv.Value.ReverseProxy && env.IsProduction())
      {
        app.UseHttpsRedirection();
      }
      // Register the Swagger generator and the Swagger UI middlewares
      app.UseSwaggerUi3WithApiExplorer(settings =>
      {
        settings.GeneratorSettings.DefaultPropertyNameHandling = 
          PropertyNameHandling.CamelCase;
        
        if(env.IsProduction())
        {
          settings.PostProcess = document => document.BasePath = "/idapi1/";
        }
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
