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
    /// <param name="action">The action to execute.</param>
    /// <param name="count">How often to execute the action.</param>
    public static void Action(Action action, int count)
    {
      for (var i = 0; i < count; i++)
      {
        action();
      }
    }
  }
}