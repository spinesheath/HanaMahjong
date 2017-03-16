// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// General Client Exception.
  /// </summary>
  [Serializable]
  public class ClientException : Exception
  {
    /// <summary>
    /// Creates a new Instance of ClientException.
    /// </summary>
    public ClientException()
    {
    }

    /// <summary>
    /// Creates a new Instance of ClientException with a message.
    /// </summary>
    public ClientException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Creates a new Instance of ClientException with a message and inner Exception.
    /// </summary>
    public ClientException(string message, Exception inner)
      : base(message, inner)
    {
    }

    /// <summary>
    /// Creates a new Instance of ClientException from a SerializationInfo and StreamingContext.
    /// </summary>
    protected ClientException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}