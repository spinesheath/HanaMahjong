// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  /// <summary>
  ///         |  Expert | Advanced
  /// Ippan   |  0      | 0
  /// Joukyuu |  0      | 1
  /// Tokujou |  1      | 0
  /// Houou   |  1      | 1
  /// </summary>
  [Flags]
  internal enum GameTypeFlag
  {
    None = 0,
    Multiplayer = 1,
    AkaNashi = 2,
    KuitanNashi = 4,
    Tonnansen = 8,
    Sanma = 16,
    Expert = 32,
    Fast = 64,
    Advanced = 128
  }
}