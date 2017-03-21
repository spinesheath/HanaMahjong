// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Spines.Hana.Clay.Models
{
  [DataContract]
  internal class PlayerModel
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string HandShorthand { get; set; }

    [DataMember]
    public string PondShorthand { get; set; }

    [DataMember]
    public string Score { get; set; }
  }
}