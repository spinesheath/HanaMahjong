// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Spines.Hana.Clay.Commands;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.ViewModels
{
  internal class MainViewModel : ViewModelBase
  {
    public MainViewModel()
    {
      Randomize = new DelegateCommand(OnRandomize);
      OnRandomize(13);
    }

    public ICommand Randomize { get; }

    public ICollection<string> TileIcons { get; } = new ObservableCollection<string>();

    public string Hand
    {
      get { return _shorthand; }
      set
      {
        if (value == _shorthand)
        {
          return;
        }
        _shorthand = value;
        try
        {
          var parser = new ShorthandParser(value);
          _currentHand = new Hand(parser);
          _invalidFormat = false;
          UpdateIcons(parser);
        }
        catch
        {
          _invalidFormat = true;
        }
        OnPropertyChanged();
        OnPropertyChanged(nameof(Shanten));
        OnPropertyChanged(nameof(InvalidFormat));
      }
    }

    public bool InvalidFormat => _invalidFormat || _shorthand == null || !_currentHand.IsValid;

    public int Shanten
    {
      get
      {
        if (_currentHand == null || !_currentHand.IsValid)
        {
          return 8;
        }
        return _currentHand.Shanten;
      }
    }

    private const string IconBasePath = @"Resources/Tiles/Perspective";

    private static readonly Dictionary<Suit, char> SuitCharacters = new Dictionary<Suit, char>
    {
      {Suit.Manzu, 'm'},
      {Suit.Pinzu, 'p'},
      {Suit.Souzu, 's'},
      {Suit.Jihai, 'j'}
    };

    private string _shorthand;
    private Hand _currentHand;
    private bool _invalidFormat;

    private void UpdateIcons(ShorthandParser parser)
    {
      var tiles = parser.Tiles;
      TileIcons.Clear();
      foreach (var tile in tiles)
      {
        var c = SuitCharacters[tile.Suit];
        var i = tile.Index + 1;
        var fileName = Path.Combine(IconBasePath, $"0{c}{i}.png");
        TileIcons.Add(Path.GetFullPath(fileName));
      }

      UkeIre.Clear();
      if (_currentHand.IsValid)
      {
        var ukeIre = _currentHand.GetUkeIre().OrderByDescending(u => u.Outs.Values.DefaultIfEmpty(0).Sum());
        foreach (var ukeIreInfo in ukeIre)
        {
          UkeIre.Add(new UkeIreViewModel(ukeIreInfo));
        }
      }
    }

    public ICollection<UkeIreViewModel> UkeIre { get; } = new ObservableCollection<UkeIreViewModel>();

    private void OnRandomize(object obj)
    {
      var count = Convert.ToInt32(obj, CultureInfo.InvariantCulture);

      var rand = new Random((int) DateTime.Now.Ticks);
      var drawn = new bool[136];

      _currentHand = new Hand();
      for (var i = 0; i < count; ++i)
      {
        var tileId = GetRandomTile(rand, drawn);
        drawn[tileId] = true;
        _currentHand.Draw(tileId / 4);
      }
      _shorthand = _currentHand.ToString();
      OnPropertyChanged(nameof(Hand));
      OnPropertyChanged(nameof(Shanten));
      OnPropertyChanged(nameof(InvalidFormat));
      var parser = new ShorthandParser(_shorthand);
      UpdateIcons(parser);
    }

    private static int GetRandomTile(Random rand, IReadOnlyList<bool> drawn)
    {
      while (true)
      {
        var n = rand.Next(136);
        if (!drawn[n])
        {
          return n;
        }
      }
    }
  }
}