// Spines.Tenhou.Client.RegistrationService.cs
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

using System.Xml.Linq;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class RegistrationService : IRegistrationService
  {
    public bool IsRegistered(string accountId)
    {
      return true;
    }

    public AccountInformation GetAccountInformation(string accountId)
    {
      var helo = XElement.Parse("<HELO uname=\"%71%77%64%66%65%72%67%68\" auth=\"20141229-cc32e3fd\" expire=\"20141230\" expiredays=\"2\" ratingscale=\"PF3=1.000000&amp;PF4=1.000000&amp;PF01C=0.582222&amp;PF02C=0.501632&amp;PF03C=0.414869&amp;PF11C=0.823386&amp;PF12C=0.709416&amp;PF13C=0.586714&amp;PF23C=0.378722&amp;PF33C=0.535594&amp;PF1C00=8.000000\"/>");
      return new AccountInformation(helo);
    }
  }
}