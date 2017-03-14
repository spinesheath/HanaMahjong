// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace Spines.Utility
{
  /// <summary>
  /// Extension Methods for validating parameters.
  /// </summary>
  public static class Validate
  {
    /// <summary>
    /// Throws an ArgumentNullException if the tested instance of a reference type is null.
    /// </summary>
    /// <typeparam name="T">The type of the instance to be tested.</typeparam>
    /// <param name="input">The instance to be tested for null.</param>
    /// <param name="argumentName">The name of argument that is to be tested.</param>
    /// <returns>The value that was passed into the method.</returns>
    public static T NotNull<T>([ValidatedNotNull] T input, string argumentName) where T : class
    {
      if (input == null)
      {
        throw new ArgumentNullException(argumentName);
      }
      return input;
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if value is smaller than min or larger than max.
    /// </summary>
    /// <param name="value">The tested value.</param>
    /// <param name="min">The smallest allowed value.</param>
    /// <param name="max">The largest allowed value.</param>
    /// <param name="argumentName">The name of the argument that is to be tested.</param>
    /// <returns>The value that was passed into the method.</returns>
    public static int InRange(int value, int min, int max, string argumentName)
    {
      if (value < min || value > max)
      {
        throw new ArgumentOutOfRangeException(argumentName, value, "Value must be between " + min + " and " + max + ".");
      }
      return value;
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if value is less than 0.
    /// </summary>
    /// <param name="value">The tested value.</param>
    /// ///
    /// <param name="argumentName">The name of the argument that is to be tested.</param>
    /// <returns>The value that was passed into the method.</returns>
    public static int NotNegative(int value, string argumentName)
    {
      if (value < 0)
      {
        throw new ArgumentOutOfRangeException(argumentName, value, "Value must not be negative.");
      }
      return value;
    }

    /// <summary>
    /// Raises the event if there are any subscribers.
    /// </summary>
    /// <param name="handler">The event handler that is raised.</param>
    /// <param name="sender">The sender of the event.</param>
    public static void InvokeSafely(EventHandler handler, object sender)
    {
      ThreadSafeCheckAndRaise(handler, h => h(sender, new EventArgs()));
    }

    /// <summary>
    /// Raises the event if there are any subscribers.
    /// </summary>
    /// <param name="handler">The event handler that is raised.</param>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The arguments of the event.</param>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    public static void InvokeSafely<TEventArgs>(EventHandler<TEventArgs> handler, object sender, TEventArgs e)
      where TEventArgs : EventArgs
    {
      ThreadSafeCheckAndRaise(handler, h => h(sender, e));
    }

    /// <summary>
    /// Raises the event if there are any subscribers.
    /// </summary>
    /// <param name="handler">The event handler that is raised.</param>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="arguments">The arguments of the event.</param>
    /// <typeparam name="TSender">The type of the sender.</typeparam>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    public static void InvokeSafely<TSender, TEventArgs>(TypedEventHandler<TSender, TEventArgs> handler, TSender sender,
      TEventArgs arguments)
      where TEventArgs : EventArgs
    {
      ThreadSafeCheckAndRaise(handler, h => h(sender, arguments));
    }

    /// <summary>
    /// Raises the event if there are any subscribers.
    /// </summary>
    /// <param name="handler">The event handler that is raised.</param>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="propertyName">The name of the changed property.</param>
    public static void InvokeSafely(PropertyChangedEventHandler handler, object sender, string propertyName)
    {
      ThreadSafeCheckAndRaise(handler, h => h(sender, new PropertyChangedEventArgs(propertyName)));
    }

    /// <summary>
    /// Copies the handler into a temporary variable to prevent race condition. This will not be optimized away in CLR 2.0.
    /// </summary>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="handler">The event handler.</param>
    /// <param name="invoker">The action that invokes the event handler. Called if the handler is not null.</param>
    private static void ThreadSafeCheckAndRaise<TEventHandler>(TEventHandler handler, Action<TEventHandler> invoker)
    {
      var h = handler;
      if (h != null)
      {
        invoker(h);
      }
    }
  }
}