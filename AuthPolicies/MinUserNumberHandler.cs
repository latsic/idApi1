using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Latsic.IdApi1.AuthPolicies
{
                                                           
  public class MinUserNumberHandler : AuthorizationHandler<MinUserNumberRequirement>
  {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   MinUserNumberRequirement requirement)
    {
      if(!context.User.HasClaim(claim => claim.Type == "UserNumber"))
      {
        return Task.CompletedTask;
      }

      int userNumber;
      if(!Int32.TryParse(context.User.FindFirst(claim => claim.Type == "UserNumber").Value, out userNumber))
      {
        return Task.CompletedTask;
      }


      if(userNumber >= requirement.MinUserNumber)
      {
        context.Succeed(requirement);
      }

      return Task.CompletedTask;
    }
  }
}