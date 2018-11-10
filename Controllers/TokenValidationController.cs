using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using IdentityModel;

using Latsic.IdApi1.Models.TransferObjects;
using Latsic.IdApi1.Configuration;

namespace Latsic.IdApi1.Controllers
{
  [Route("[controller]/[action]")]
  [ApiController]
  public class TokenValidationConfigController : ControllerBase
  {
    private readonly ApiSettings _apiSettings;
    private readonly ITokenValidationConfig _tokenValidationConfig;

    public TokenValidationConfigController(
      IOptions<ApiSettings> apiSettings, ITokenValidationConfig tokenValidationConfig)
    {
      _apiSettings = apiSettings.Value;
      _tokenValidationConfig = tokenValidationConfig;
    }

    // GET api/values
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [AllowAnonymous]
    public ActionResult<ClockSkew> ClockSkew()
    {
      return Ok(new ClockSkew
      {
        TimeSpan = (int)_tokenValidationConfig.JwtValidationClockSkew.TotalSeconds
      });
    }
    // GET api/values
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [Authorize]
    public ActionResult<ClockSkew> ClockSkew([FromBody]ClockSkew clockSkew)
    {
      if(clockSkew.TimeSpan < 0) {
        
        return BadRequest(new {
          message = $"timespan value of {clockSkew.TimeSpan} is invalid, must not be < 0"
        });
      }
      if(clockSkew.TimeSpan > 500) {
        return BadRequest(new {
          message = $"timespan value of {clockSkew.TimeSpan} is invalid, must not be > 500"
        });
      }

      _tokenValidationConfig.JwtValidationClockSkew = TimeSpan.FromSeconds(clockSkew.TimeSpan);

      return Ok(clockSkew);
    }
  }
}
