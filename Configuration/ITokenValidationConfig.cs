using System;
using IdentityServer4.AccessTokenValidation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace Latsic.IdApi1.Configuration {
  public interface ITokenValidationConfig {
    TimeSpan JwtValidationClockSkew { get; set; }
    TokenValidationParameters TokenValidationParameters { set; }
  }
}