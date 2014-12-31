// Spines.Tenhou.Client.ClientException.cs
// 
// Copyright (C) 2014  Johannes Heckl
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