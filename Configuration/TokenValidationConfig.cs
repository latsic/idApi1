
using System;
using Microsoft.IdentityModel.Tokens;

namespace Latsic.IdApi1.Configuration {
  public class TokenValidationConfig : ITokenValidationConfig {

    private TokenValidationParameters _tokenValidationParameters;
    private readonly TimeSpan _clockSkewDefault = TimeSpan.FromSeconds(300);

    public TokenValidationParameters TokenValidationParameters { 
      set {
        _tokenValidationParameters = value;
      }
    }

    public TimeSpan JwtValidationClockSkew {
      get {
        return _tokenValidationParameters != null && _tokenValidationParameters.ClockSkew != null
          ? _tokenValidationParameters.ClockSkew
          :  _clockSkewDefault;
      }
      set {
        _tokenValidationParameters.ClockSkew = value;
      }
    }
  }
}