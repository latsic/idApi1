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
  [Authorize(Policy = "ApiAccess")]
  public class TestController : ControllerBase
  {
    private readonly ApiSettings _apiSettings;

    public TestController(IOptions<ApiSettings> apiSettings)
    {
      _apiSettings = apiSettings.Value;
    }

    
    [HttpGet]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    public ActionResult<Info> Everybody()
    {
      return Ok(new Info
      {
        AccessRequirement = "none",
        ClaimsNeeded = "none"
      });
    }
   
    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize]
    public ActionResult<Info> EverybodyWithAToken()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = "None"
      });
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "AgeAtLeast16")]
    public ActionResult<Info> AgeAtLeast16()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"ApiAcess, {JwtClaimTypes.BirthDate} and calculated age >= 16"
      });
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "AgeAtLeast18")]
    public ActionResult<Info> AgeAtLeast18()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"ApiAcess, {JwtClaimTypes.BirthDate} and calculated age >= 18"
      });
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "AgeAtLeast21")]
    public ActionResult<Info> AgeAtLeast21()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"ApiAcess, {JwtClaimTypes.BirthDate} and calculated age >= 21"
      });
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "Admin")]
    public ActionResult<Object> AdminRole()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"ApiAcess, {ClaimTypes.Role} with a value of Admin"
      });
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "Admin")]
    [Authorize(Policy = "AgeAtLeast21")]
    public ActionResult<Info> AdminRoleAgeAtLeast21()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"ApiAcess, {JwtClaimTypes.BirthDate} and calculated age >= 21 " +
          $"and {ClaimTypes.Role} with a value of Admin"
      });
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UserNumberAtLeast20")]
    public ActionResult<Info> UserNumberAtLeast20()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"ApiAcess, UserNumber with a value >= 20"
      });
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UserNumberAtLeast20")]
    [Authorize(Policy = "AgeAtLeast18")]
    public ActionResult<Info> UserNumberAtLeast20AgeAtLeast18()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"ApiAcess, {JwtClaimTypes.BirthDate} and calculated age >= 20 " +
          $" and a UserNumber >= 20"
      });
    }
  }
}
