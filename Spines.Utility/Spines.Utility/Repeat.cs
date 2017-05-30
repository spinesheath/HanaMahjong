// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Utility
{
  /// <summary>
  /// Allows repeated execution of an action.
  /// </summary>
  public static class Repeat
  {
    /// <summary>
    /// Repeats the action count times.
    /// </summary>
    /// <param name="toRepeat">The action to execute.</param>
    /// <param name="count">How often to execute the action.</param>
    public static void Action(Action toRepeat, int count)
    {
      Validate.NotNull(toRepeat, nameof(toRepeat));
      Validate.NotNegative(count, nameof(count));
      for (var i = 0; i < count; i++)
      {
        toRepeat();
      }
    }
  }
}