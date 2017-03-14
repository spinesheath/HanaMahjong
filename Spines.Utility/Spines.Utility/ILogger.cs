// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace Spines.Utility
{
  /// <summary>
  /// Interface for logging.
  /// </summary>
  public interface ILogger
  {
    /// <summary>
    /// Logs the message at trace level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Trace([Localizable(false)] string message);
  }
}