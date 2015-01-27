// Spines.Tenhou.Client.LocalMahjongServer.cs
// 
// Copyright (C) 2015  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client
{
  internal class LocalMahjongServer : ITenhouServer
  {
    private readonly ISeedGenerator _seedGenerator;

    public LocalMahjongServer(ISeedGenerator seedGenerator)
    {
      _seedGenerator = seedGenerator;
    }

    public void RaiseConnected()
    {
      EventUtility.CheckAndRaise(Connected, this, new EventArgs());
    }

    private readonly IList<AccountInformation> _accounts = new List<AccountInformation>();

    public void RegisterAccount(AccountInformation accountInformation)
    {
      if (_accounts.All(a => a.UserName != accountInformation.UserName))
      {
        _accounts.Add(accountInformation);
      }
    }

    private void LogInUser(XElement message)
    {
      var connectingAccount = new AccountInformation(message);
      var registeredAccount = _accounts.FirstOrDefault(a => a.UserName == connectingAccount.UserName);
      if(null == registeredAccount)
        return;

      var uname = new XAttribute("uname", "%71%77%64%66%65%72%67%68");
      var auth = new XAttribute("auth", "20141229-cc32e3fd");
      var expire = new XAttribute("expire", "20141230");
      var days = new XAttribute("expiredays", "2");
      var scale = new XAttribute("ratingscale",
        "PF3=1.000000&PF4=1.000000&PF01C=0.582222&PF02C=0.501632&PF03C=0.414869&PF11C=0.823386&PF12C=0.709416&PF13C=0.586714&PF23C=0.378722&PF33C=0.535594&PF1C00=8.000000");
      EventUtility.CheckAndRaise(Receive, this, new ReceivedMessageEventArgs(new XElement("HELO", uname, auth, expire, days, scale)));
    }

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Send(XElement message)
    {
      if (message.Name == "HELO")
      {
        LogInUser(message);
      }
    }

    public event EventHandler<ReceivedMessageEventArgs> Receive;
    public event EventHandler<EventArgs> Connected;
  }
}