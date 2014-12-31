// Spines.Tenhou.Client.Authenticator.cs
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
using System.Globalization;

namespace Spines.Tenhou.Client
{
  internal static class Authenticator
  {
    private const char AuthenticationStringSplitter = '-';

    private static readonly int[] TranslationTable =
    {
      63006, 9570, 49216, 45888, 9822, 23121, 59830, 51114, 54831, 4189, 580, 5203, 42174, 59972, 55457, 59009, 59347,
      64456, 8673, 52710, 49975, 2006, 62677, 3463, 17754, 5357
    };

    public static string Transform(string authenticationString)
    {
      var parts = authenticationString.Split(new[] {AuthenticationStringSplitter});
      if (parts.Length != 2 || parts[0].Length != 8 || parts[1].Length != 8)
      {
        return authenticationString;
      }
      return parts[0] + AuthenticationStringSplitter + CreatePostfix(parts[0], parts[1]);
    }

    private static string CreatePostfix(string p0, string p1)
    {
      var tableIndex = GetTableIndex(p0);
      var a = TranslationTable[tableIndex] ^ ConvertHexToInt(p1.Substring(0, 4));
      var b = TranslationTable[tableIndex + 1] ^ ConvertHexToInt(p1.Substring(4, 4));
      return ConvertIntToHex4(a) + ConvertIntToHex4(b);
    }

    private static int GetTableIndex(string p0)
    {
      return ConvertDecimalToInt("2" + p0.Substring(2, 6)) % (12 - ConvertDecimalToInt(p0.Substring(7, 1))) * 2;
    }

    private static int ConvertDecimalToInt(string s)
    {
      return Convert.ToInt32(s, CultureInfo.InvariantCulture);
    }

    private static int ConvertHexToInt(string s)
    {
      return int.Parse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
    }

    private static string ConvertIntToHex4(int i)
    {
      return i.ToString("x4", CultureInfo.InvariantCulture);
    }
  }
}