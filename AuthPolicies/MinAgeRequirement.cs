using Microsoft.AspNetCore.Authorization;

namespace Latsic.IdApi1.AuthPolicies
{
  public class MinAgeRequirement : IAuthorizationRequirement
  {
    public int MinimumAge { get; private set; }

    public MinAgeRequirement(int minimumAge)
    {
      MinimumAge = minimumAge;
    }
  }
}