// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// The result of a call offer.
  /// </summary>
  internal enum CallResult
  {
    /// <summary>
    /// The call offer was ignored.
    /// </summary>
    Ignore,

    /// <summary>
    /// The call was made.
    /// </summary>
    Call,

    /// <summary>
    /// The hand was won off the call.
    /// </summary>
    Ron
  }
}