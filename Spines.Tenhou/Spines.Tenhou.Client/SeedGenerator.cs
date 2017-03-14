// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Tenhou.Client
{
  /// <summary>
  /// Creates a random seed similar to the ones created by tenhou.net.
  /// </summary>
  internal class SeedGenerator : ISeedGenerator
  {
    /// <summary>
    /// Creates a random seed similar to the ones created by tenhou.net.
    /// </summary>
    /// <returns>A random seed.</returns>
    public string CreateSeed()
    {
      var t = new char[SeedLength];
      for (var i = 0; i < SeedLength; ++i)
      {
        t[i] = AllowedCharacters[_random.Next(AllowedCharacters.Length)];
      }
      return Prefix + "," + new string(t);
    }

    private const string AllowedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+/";
    private const int SeedLength = 3328;
    private const string Prefix = "mt19937ar-sha512-n288-base64";
    private readonly Random _random = new Random();
  }
}