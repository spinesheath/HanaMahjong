// Spines.Tenhou.Client.MatchInformation.cs
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

using Spines.Utility;
using System.Xml.Linq;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Information about a match.
  /// </summary>
  public class MatchInformation
  {
    /// <summary>
    /// Instantiates a new instance of MatchInformation.
    /// </summary>
    internal MatchInformation(XElement message)
    {
      MatchType = new MatchType(message.Attribute("type").Value);
      Lobby = InvariantConvert.ToInt32(message.Attribute("lobby").Value);
      MatchId = message.Attribute("gpid").Value;
    }

    /// <summary>
    /// The type of the match.
    /// </summary>
    public MatchType MatchType { get; private set; }

    /// <summary>
    /// The lobby for the match.
    /// </summary>
    public int Lobby { get; private set; }

    /// <summary>
    /// The match id, required for rejoining a match after losing connection.
    /// </summary>
    public string MatchId { get; private set; }
  }
}