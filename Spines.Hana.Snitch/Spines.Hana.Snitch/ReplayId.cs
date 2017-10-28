// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spines.Hana.Snitch
{
  internal static class ReplayId
  {
    public static IEnumerable<ReplayData> GetData(IEnumerable<string> lines)
    {
      var matches = lines.Select(line => Regex.Match(line));
      return matches.Where(match => match.Success).Select(MatchToReplayData);
    }

    // YYYYMMDDHHgm-0000-0000-01234567
    // YYYYMMDDHHgm-0000-0000-x0123456789ab
    private static readonly Regex Regex = new Regex(@"file=(\d{10}gm(?:%2D|-)\d{4}(?:%2D|-)\d{4}(?:%2D|-)(?:[\da-f]{8}|x[\da-f]{12})).*oya=(\d).*sc=([\d\.,-]*)");

    private static readonly int[] TranslationTable =
    {
      22136, 52719, 55146, 42104, 59591, 46934, 9248, 28891, 49597,
      52974, 62844, 4015, 18311, 50730, 43056, 17939, 64838, 38145,
      27008, 39128, 35652, 63407, 65535, 23473, 35164, 55230, 27536,
      4386, 64920, 29075, 42617, 17294, 18868, 2081
    };

    private static readonly string[] ToHexTable = "0123456789abcdef".Select(c => new string(c, 1)).ToArray();

    private static ReplayData MatchToReplayData(Match match)
    {
      var id = Normalize(match.Groups[1].Value);
      var oya = ToInt(match.Groups[2].Value);
      var scores = match.Groups[3].Value.Split(',');
      var playerCount = scores.Length / 2;
      var position = (playerCount - oya) % playerCount;
      return new ReplayData(id, position);
    }

    private static string Normalize(string id)
    {
      var parts = id.Split(new[] {"%2D", "-"}, StringSplitOptions.RemoveEmptyEntries);
      if (parts.Length != 4)
      {
        return id;
      }
      if (parts[3][0] != 'x')
      {
        return string.Join("-", parts);
      }
      var a = UnHex4(parts[3].Substring(1, 4));
      var b = UnHex4(parts[3].Substring(5, 4));
      var c = UnHex4(parts[3].Substring(9, 4));
      var d = 0;
      if (string.Compare(parts[0], "2010041111gm", StringComparison.Ordinal) >= 0)
      {
        d = Convert.ToInt32("3" + parts[0].Substring(4, 6)) % (17 * 2 - Convert.ToInt32(parts[0].Substring(9, 1)) - 1);
      }
      parts[3] = HexToString4(a ^ b ^ TranslationTable[d + 0]) + HexToString4(b ^ TranslationTable[d + 0] ^ c ^ TranslationTable[d + 1]);
      return string.Join("-", parts);
    }

    private static int ToInt(string value)
    {
      return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static string HexToString4(int i)
    {
      return ToHexTable[i >> 12 & 15] + ToHexTable[i >> 8 & 15] + ToHexTable[i >> 4 & 15] + ToHexTable[i >> 0 & 15];
    }

    private static int UnHex4(string s)
    {
      return UnHex(s[0]) * 4096 | UnHex(s[1]) * 256 | UnHex(s[2]) * 16 | UnHex(s[3]) * 1;
    }

    private static int UnHex(int i)
    {
      if (48 <= i && i <= 57)
      {
        return i - 48;
      }
      if (65 <= i && i <= 90)
      {
        return i - 55;
      }
      if (97 <= i && i <= 122)
      {
        return i - 87;
      }
      return 0;
    }
  }
}