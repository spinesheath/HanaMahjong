// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.ViewModels
{
  internal class UkeIreViewModel : ViewModelBase
  {
    public UkeIreViewModel(UkeIreInfo ukeIre)
    {
      DiscardIcon = ukeIre.Discard == null ? null : GetFileName(ukeIre.Discard.Value);
      foreach (var tile in ukeIre.Outs.Keys)
      {
        var fileName = GetFileName(tile);
        TileIcons.Add(fileName);
      }
      Count = ukeIre.Outs.Values.DefaultIfEmpty(0).Sum();
    }

    public int Count { get; }

    public string DiscardIcon { get; }

    public ICollection<string> TileIcons { get; } = new ObservableCollection<string>();

    private const string IconBasePath = @"Resources/Tiles/Perspective";

    private static readonly Dictionary<Suit, char> SuitCharacters = new Dictionary<Suit, char>
    {
      {Suit.Manzu, 'm'},
      {Suit.Pinzu, 'p'},
      {Suit.Souzu, 's'},
      {Suit.Jihai, 'j'}
    };

    private static string GetFileName(Tile tile)
    {
      var c = SuitCharacters[tile.Suit];
      var i = tile.Index + 1;
      return Path.GetFullPath(Path.Combine(IconBasePath, $"0{c}{i}.png"));
    }
  }
}