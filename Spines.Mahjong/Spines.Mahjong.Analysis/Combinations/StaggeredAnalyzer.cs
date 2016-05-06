// Spines.Mahjong.Analysis.StaggeredAnalyzer.cs
// 
// Copyright (C) 2016  Johannes Heckl
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

using System.Collections.Generic;

namespace Spines.Mahjong.Analysis.Combinations
{
  internal class StaggeredAnalyzer : IStaggeredAnalyzer
  {
    private readonly List<int> _concealed;
    private readonly IStaggeredAnalyzer _next;
    private readonly ProtoGroup _protoGroup;
    private readonly List<int> _used;

    public StaggeredAnalyzer(ProtoGroup protoGroup, IStaggeredAnalyzer next, List<int> concealed, List<int> used)
    {
      _protoGroup = protoGroup;
      _next = next;
      _concealed = concealed;
      _used = used;
    }

    public IEnumerable<Arrangement> Analyze(Arrangement arrangement, int currentTileType)
    {
      foreach (var arr in _next.Analyze(arrangement, 0))
      {
        yield return arr;
      }
      if (_protoGroup.IsJantou && arrangement.Jantou >= 1 || !_protoGroup.IsJantou && arrangement.Mentsu >= 4)
      {
        yield return arrangement;
      }
      else
      {
        foreach (var a in AddProtoGroup(arrangement, currentTileType))
        {
          yield return a;
        }
      }
    }

    private IEnumerable<Arrangement> AddProtoGroup(Arrangement arrangement, int currentTileType)
    {
      for (var offset = currentTileType; offset < 9; ++offset)
      {
        if (!_protoGroup.CanInsert(_concealed, _used, offset))
        {
          continue;
        }
        _protoGroup.Insert(_concealed, _used, offset);
        var current = _protoGroup.IsJantou
          ? arrangement.AddJantou(_protoGroup.Value)
          : arrangement.AddMentsu(_protoGroup.Value);
        foreach (var arr in Analyze(current, offset))
        {
          yield return arr;
        }
        _protoGroup.Remove(_concealed, _used, offset);
      }
    }
  }
}