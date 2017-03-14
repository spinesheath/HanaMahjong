// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Text;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// A username as used by tenhou.net.
  /// </summary>
  public class UserName
  {
    /// <summary>
    /// The plain text name.
    /// </summary>
    public string Name
    {
      get { return Decode(EncodedName); }
    }

    private static string Decode(string encodedName)
    {
      var encodedCharacters = encodedName.Split(new[] {'%'}, StringSplitOptions.RemoveEmptyEntries);
      var decodedCharacters = encodedCharacters.Select(c => Convert.ToByte(c, 16)).ToArray();
      return new UTF8Encoding().GetString(decodedCharacters);
    }

    internal UserName(string encodedName)
    {
      EncodedName = encodedName;
    }

    internal string EncodedName { get; }
  }
}