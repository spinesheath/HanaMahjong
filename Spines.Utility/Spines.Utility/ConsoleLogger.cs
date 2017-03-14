// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace Spines.Utility
{
  /// <summary>
  /// Logger that writes to the console.
  /// </summary>
  public class ConsoleLogger : ILogger
  {
    /// <summary>
    /// Logs the message at trace level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Trace([Localizable(false)] string message)
    {
      Console.WriteLine(message);
    }
  }
}