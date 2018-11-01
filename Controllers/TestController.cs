﻿using System;
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
  public class TestController : ControllerBase
  {
    private readonly ApiSettings _apiSettings;

    public TestController(IOptions<ApiSettings> apiSettings)
    {
      _apiSettings = apiSettings.Value;
    }

    // GET api/values
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
    // GET api/values
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

    // GET api/values/5
    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "AgeAtLeast16")]
    public ActionResult<Info> AgeAtLeast16()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"{JwtClaimTypes.BirthDate} and calculated age >= 16"
      });
    }

    // GET api/values/5
    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "AgeAtLeast18")]
    public ActionResult<Info> AgeAtLeast18()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"{JwtClaimTypes.BirthDate} and calculated age >= 18"
      });
    }

    // GET api/values/5
    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "AgeAtLeast21")]
    public ActionResult<Info> AgeAtLeast21()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"{JwtClaimTypes.BirthDate} and calculated age >= 21"
      });
    }

    // GET api/values/5
    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "Admin")]
    public ActionResult<Object> AdminRole()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"{ClaimTypes.Role} with a value of Admin"
      });
    }

    // GET api/values/5
    [HttpGet]
    [ProducesResponseType(200)]
    [Authorize(Policy = "Admin")]
    [Authorize(Policy = "AgeAtLeast21")]
    public ActionResult<Info> AdminRoleAgeAtLeast21()
    {
      return Ok(new Info
      {
        AccessRequirement = $"Access Token from Authority at {_apiSettings.AuthAuthorityUrl}",
        ClaimsNeeded = $"{JwtClaimTypes.BirthDate} and calculated age >= 21 " +
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
        ClaimsNeeded = $"UserNumber with a value >= 20"
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
        ClaimsNeeded = $"{JwtClaimTypes.BirthDate} and calculated age >= 20 " +
          $" and a UserNumber >= 20"
      });
    }
  }
}
