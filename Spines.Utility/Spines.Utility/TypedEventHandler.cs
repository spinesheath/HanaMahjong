// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Utility
{
  /// <summary>
  /// Strongly typed event handler.
  /// </summary>
  /// <param name="sender">The sender of the event.</param>
  /// <param name="arguments">The arguments of the event.</param>
  /// <typeparam name="TSender">The type of the sender.</typeparam>
  /// <typeparam name="TEventArgs">The type of the arguments.</typeparam>
  public delegate void TypedEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs arguments);
}