// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Spines.Hana.Clay.Commands;
using Spines.Hana.Clay.Controls;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.ViewModels
{
  internal class MainViewModel : ViewModelBase
  {
    public MainViewModel()
    {
      Draw = new DelegateCommand(OnDraw);
      Discard = new DelegateCommand(OnDiscard);
      Randomize = new DelegateCommand(OnRandomize);
      Export = new DelegateCommand(OnExport);
      OnRandomize(13);
    }

    public ICommand Randomize { get; }

    public ICommand Draw { get; }

    public ICommand Discard { get; }

    public ICommand Export { get; }

    public ICollection<UkeIreViewModel> UkeIre { get; } = new ObservableCollection<UkeIreViewModel>();

    public ICollection<Tile> Tiles { get; } = new ObservableCollection<Tile>();

    public ICollection<Meld> Melds { get; } = new ObservableCollection<Meld>();

    public ICollection<Tile> Pond { get; } = new ObservableCollection<Tile>();

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

    private void OnExport(object obj)
    {
      try
      {
        var view = new HandDisplay {Melds = Melds, Tiles = Tiles};
        var grid = new Grid();
        grid.Children.Add(view);
        view.ApplyTemplate();
        view.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        view.Arrange(new Rect(view.DesiredSize));
        var result = new RenderTargetBitmap((int) view.ActualWidth, (int) view.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        var drawingvisual = new DrawingVisual();
        using (var context = drawingvisual.RenderOpen())
        {
          context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), view.DesiredSize));
          context.Close();
        }
        result.Render(drawingvisual);
        Clipboard.SetImage(result);
      }
      catch
      {
        // ignored
      }
    }

    private void OnDiscard(object obj)
    {
      if (!_currentHand.CanDiscard)
      {
        return;
      }

      var tile = (Tile) obj;
      _currentHand.Discard(tile);

      UpdateIcons(new ShorthandParser(_currentHand.ToString()));
      OnPropertyChanged(nameof(Shanten));
    }

    private void OnDraw(object obj)
    {
      if (null == _currentHand)
      {
        return;
      }
      if (!_currentHand.CanDraw)
      {
        if (_currentHand.Shanten == -1)
        {
          return;
        }
        _currentHand.Discard();
      }
      var drawable = _currentHand.GetDrawableTileTypes();
      var map = drawable.SelectMany((d, i) => Enumerable.Repeat(i, d)).ToList();
      if (map.Count == 0)
      {
        return;
      }
      var rand = new Random((int) DateTime.Now.Ticks);
      var toDraw = map[rand.Next(map.Count)];
      _currentHand.Draw(toDraw);

      UpdateIcons(new ShorthandParser(_currentHand.ToString()));
      OnPropertyChanged(nameof(Shanten));
    }

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

      Pond.Clear();
      foreach (var tile in _currentHand.GetPond())
      {
        Pond.Add(tile);
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