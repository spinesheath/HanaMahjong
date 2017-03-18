// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

    public ICollection<UkeIreViewModel> UkeIre { get; } = new ObservableCollection<UkeIreViewModel>();

    public string Shorthand
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
          UkeIre.Clear();
        }
        OnPropertyChanged();
        OnPropertyChanged(nameof(Shanten));
        OnPropertyChanged(nameof(InvalidFormat));
      }
    }

    public ICollection<Tile> Tiles { get; } = new ObservableCollection<Tile>();

    public ICollection<Meld> Melds { get; } = new ObservableCollection<Meld>();

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

    private string _shorthand;
    private Hand _currentHand;
    private bool _invalidFormat;

    private void UpdateIcons(ShorthandParser parser)
    {
      Tiles.Clear();
      foreach (var tile in parser.Tiles)
      {
        Tiles.Add(tile);
      }
      Melds.Clear();
      foreach (var meld in parser.Melds)
      {
        Melds.Add(meld);
      }

      UkeIre.Clear();
      if (!_currentHand.IsValid)
      {
        return;
      }
      var ukeIre = _currentHand.GetUkeIre().OrderByDescending(u => u.Outs.Values.DefaultIfEmpty(0).Sum());
      foreach (var ukeIreInfo in ukeIre)
      {
        UkeIre.Add(new UkeIreViewModel(ukeIreInfo));
      }
    }

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
      OnPropertyChanged(nameof(Shorthand));
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