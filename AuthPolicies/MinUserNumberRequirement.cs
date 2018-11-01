using Microsoft.AspNetCore.Authorization;

namespace Latsic.IdApi1.AuthPolicies
{
  public class MinUserNumberRequirement : IAuthorizationRequirement
  {
    public int MinUserNumber { get; private set; }

    public MinUserNumberRequirement(int minUserNumber)
    {
      MinUserNumber = minUserNumber;
    }
  }
}
