// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Spines.MachiKae.Commands;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.MachiKae.ViewModels
{
  internal class MainViewModel : ViewModelBase
  {
    public MainViewModel()
    {
      Randomize = new DelegateCommand(OnRandomize);
    }

    public ICommand Randomize { get; }

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
          _currentHand = new Hand(value);
          _invalidFormat = false;
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

    private string _shorthand;
    private Hand _currentHand;
    private bool _invalidFormat;

    private void OnRandomize(object obj)
    {
      var rand = new Random((int) DateTime.Now.Ticks);
      var drawn = new bool[136];

      _currentHand = new Hand();
      for (var i = 0; i < 13; ++i)
      {
        var tileId = GetRandomTile(rand, drawn);
        drawn[tileId] = true;
        _currentHand.Draw(tileId / 4);
      }
      _shorthand = _currentHand.ToString();
      OnPropertyChanged(nameof(Hand));
      OnPropertyChanged(nameof(Shanten));
      OnPropertyChanged(nameof(InvalidFormat));
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