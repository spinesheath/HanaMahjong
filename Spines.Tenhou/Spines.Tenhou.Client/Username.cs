// Spines.Tenhou.Client.Username.cs
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
using System.Linq;
using System.Text;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A username as used by tenhou.net.
  /// </summary>
  public class UserName
  {
    internal UserName(string encodedName)
    {
      EncodedName = encodedName;
    }

    internal string EncodedName { get; private set; }

    /// <summary>
    /// The plain text name.
    /// </summary>
    public string Name
    {
      get { return Decode(EncodedName); }
    }

    private static string Decode(string encodedName)
    {
      var encodedCharacters = encodedName.Split(new[] {'%'}, StringSplitOptions.RemoveEmptyEntries);
      var decodedCharacters = encodedCharacters.Select(c => Convert.ToByte(c, 16)).ToArray();
      return new UTF8Encoding().GetString(decodedCharacters);
    }
  }
}