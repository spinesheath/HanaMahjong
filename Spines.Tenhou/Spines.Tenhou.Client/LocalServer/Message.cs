// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.Linq;

namespace Spines.Tenhou.Client.LocalServer
{
  internal class Message
  {
    public Message(string senderId, XElement content)
    {
      Content = content;
      SenderId = senderId;
    }

    public XElement Content { get; private set; }
    public string SenderId { get; private set; }
  }
}