// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Utility
{
  /// <summary>
  /// Dummy implementation of ILogger that doesn't actually log anything.
  /// </summary>
  public class NullLogger : ILogger
  {
    /// <summary>
    /// Logs the message at trace level. Doesn't actually log anything.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Trace(string message)
    {
    }
  }
}