// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  [DataContract]
  internal class Game
  {
    [DataMember(Name = "oya")]
    public int Oya { get; set; }

    [DataMember(Name = "wall")]
    public List<int> Wall { get; } = new List<int>();

    [DataMember(Name = "dice")]
    public List<int> Dice { get; } = new List<int>();

    [DataMember(Name = "actions")]
    public List<int> Actions { get; } = new List<int>();
  }
}