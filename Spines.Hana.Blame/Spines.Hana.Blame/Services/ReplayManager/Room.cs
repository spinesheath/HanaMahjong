// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  internal class Room
  {
    public string Name { get; }

    public static IEnumerable<Room> Rooms => AllRooms.Values;

    public static Room Parse(GameTypeFlag flags)
    {
      var relevantFlags = flags & RelevantFlags;
      return AllRooms.ContainsKey(relevantFlags) ? AllRooms[relevantFlags] : null;
    }

    private Room(GameTypeFlag flags, string name)
    {
      Name = name;
      _flags = flags;
    }

    private const GameTypeFlag RelevantFlags = GameTypeFlag.Advanced | GameTypeFlag.Expert;

    private readonly GameTypeFlag _flags;

    private static readonly Room Ippan = new Room(GameTypeFlag.None, nameof(Ippan));
    private static readonly Room Joukyuu = new Room(GameTypeFlag.Advanced, nameof(Joukyuu));
    private static readonly Room Tokujou = new Room(GameTypeFlag.Expert, nameof(Tokujou));
    private static readonly Room Houou = new Room(GameTypeFlag.Advanced | GameTypeFlag.Expert, nameof(Houou));

    private static readonly Dictionary<GameTypeFlag, Room> AllRooms = new Dictionary<GameTypeFlag, Room>
    {
      {Ippan._flags, Ippan},
      {Joukyuu._flags, Joukyuu},
      {Tokujou._flags, Tokujou},
      {Houou._flags, Houou}
    };
  }
}