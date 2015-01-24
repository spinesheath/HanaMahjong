// Spines.Utility.EventUtility.cs
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

namespace Spines.Utility
{
  /// <summary>
  /// Utility Methods for eventing.
  /// </summary>
  public static class EventUtility
  {
    /// <summary>
    /// Raises the event if there are any subscribers.
    /// </summary>
    /// <param name="handler">The event handler that is raised.</param>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The arguments of the event.</param>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    public static void CheckAndRaise<TEventArgs>(EventHandler<TEventArgs> handler, object sender, TEventArgs e)
      where TEventArgs : EventArgs
    {
      // Copy into a temporary variable to prevent race condition. This will not be optimized away in CLR 2.0.
      var h = handler;
      if (h != null)
      {
        h(sender, e);
      }
    }
  }
}