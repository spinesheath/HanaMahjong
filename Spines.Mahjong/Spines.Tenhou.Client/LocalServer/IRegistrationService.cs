// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tenhou.Client.LocalServer
{
  internal interface IRegistrationService
  {
    bool IsRegistered(string accountId);
    AccountInformation GetAccountInformation(string accountId);
  }
}