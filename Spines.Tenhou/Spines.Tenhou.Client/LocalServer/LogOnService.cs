// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Xml.Linq;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class LogOnService
  {
    public bool TryLogOn(string accountId, LocalConnection connection)
    {
      lock (_accounts)
      {
        if (_accounts.ContainsKey(accountId))
        {
          return false;
        }
        _accounts.Add(accountId, connection);
        return true;
      }
    }

    public void Send(string accountId, XElement message)
    {
      lock (_accounts)
      {
        if (_accounts.ContainsKey(accountId))
        {
          _accounts[accountId].Receive(message);
        }
      }
    }

    public void LogOff(string accountId)
    {
      lock (_accounts)
      {
        _accounts.Remove(accountId);
      }
    }

    private readonly IDictionary<string, LocalConnection> _accounts = new Dictionary<string, LocalConnection>();
  }
}