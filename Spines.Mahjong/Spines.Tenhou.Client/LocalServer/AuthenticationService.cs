// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client.LocalServer
{
  internal class AuthenticationService : IAuthenticationService
  {
    public string GetAuthenticationString(string accountId)
    {
      return AuthenticationString;
    }

    public bool IsValid(string accountId, string authenticatedString)
    {
      return Authenticator.Transform(AuthenticationString) == authenticatedString;
    }

    private const string AuthenticationString = "20141229-cc32e3fd";
  }
}