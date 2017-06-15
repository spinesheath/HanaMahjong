// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Spines.Hana.Blame.Collection
{
  internal class Downloader
  {
    public Downloader()
    {
      var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      var configPath = Path.Combine(local, @"C-EGG\tenhou\130\config.ini");
      var lines = File.ReadAllLines(configPath);
      foreach (var line in lines)
      {
        var match = ReplayRegex.Match(line);
        if (!match.Success)
        {
          continue;
        }
        var id = match.Groups[1].Value;
        var url = GenerateDownloadUrl(id);
        LoadAsync(url, id);
      }
    }

    private static readonly HttpClient Client = new HttpClient();

    private static async void LoadAsync(string url, string id)
    {
      var path = $@"D:\replays\{id}.xml";
      if (File.Exists(path))
      {
        return;
      }
      var x = await Client.GetStringAsync(url);
      File.WriteAllText(path, x);
    }

    private static readonly Regex ReplayRegex =
      new Regex("^[0-9]+=file=([^&]*)&un0=([^&]*)&un1=([^&]*)&un2=([^&]*)&un3=([^&]*)&oya=([0-3])&type=([0-9]*)" + 
        @"&sc=([0-9\.-]*),([0-9\.-]*),([0-9\.-]*),([0-9\.-]*),([0-9\.-]*),([0-9\.-]*),([0-9\.-]*),([0-9\.-]*)$");

    private static readonly int[] TranslationTable =
    {
      22136, 52719, 55146, 42104, 59591, 46934, 9248, 28891, 49597,
      52974, 62844, 4015, 18311, 50730, 43056, 17939, 64838, 38145,
      27008, 39128, 35652, 63407, 65535, 23473, 35164, 55230, 27536,
      4386, 64920, 29075, 42617, 17294, 18868, 2081
    };

    private static readonly string[] ToHexTable = "0123456789abcdef".Select(c => new string(c, 1)).ToArray();

    private static string GenerateDownloadUrl(string id)
    {
      var parts = id.Split('-');
      if (parts.Length != 4)
      {
        return id;
      }
      if (parts[3][0] != 'x')
      {
        return "http://tenhou.net/0/log/?" + id;
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
      return "http://tenhou.net/0/log/?" + string.Join("-", parts);
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