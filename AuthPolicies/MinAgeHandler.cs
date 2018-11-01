using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;

namespace Latsic.IdApi1.AuthPolicies
{
  public class MinAgeHandler : AuthorizationHandler<MinAgeRequirement>
  {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   MinAgeRequirement requirement)
    {
      if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth || c.Type == JwtClaimTypes.BirthDate))
      {
        return Task.CompletedTask;
      }

      var dateOfBirth = Convert.ToDateTime(
          context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth || c.Type == JwtClaimTypes.BirthDate).Value);

      int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
      if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
      {
        calculatedAge--;
      }

      if (calculatedAge >= requirement.MinimumAge)
      {
        context.Succeed(requirement);
      }

      //TODO: Use the following if targeting a version of
      //.NET Framework older than 4.6:
      //      return Task.FromResult(0);
      return Task.CompletedTask;
    }
  }
}