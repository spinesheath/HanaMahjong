// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Spines.Hana.Clay.Commands;
using Spines.Hana.Clay.Controls;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

namespace Spines.Hana.Clay.ViewModels
{
  internal class UkeIreMainViewModel : ViewModelBase
  {
    public UkeIreMainViewModel()
    {
      Calculation = new AsyncCommand<UkeIreResult>(Calculate);

      Draw = new DelegateCommand(OnDraw);
      Discard = new DelegateCommand(OnDiscard);
      Randomize = new DelegateCommand(OnRandomize);
      Export = new DelegateCommand(OnExport);

      OnRandomize(13);
    }

    public IAsyncCommand<UkeIreResult> Calculation { get; }

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
          _currentHand = new HandCalculator(parser);
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

    public int ImproveAmount
    {
      get { return _improveAmount; }
      set
      {
        if (_improveAmount == value)
        {
          return;
        }
        _improveAmount = value;
        OnPropertyChanged();
        Calculation.ExecuteAsync(null);
      }
    }

    public int DrawCount
    {
      get { return _drawCount; }
      set
      {
        if (_drawCount == value)
        {
          return;
        }
        _drawCount = value;
        OnPropertyChanged();
        Calculation.ExecuteAsync(null);
      }
    }

    private string _shorthand;
    private HandCalculator _currentHand;
    private bool _invalidFormat;
    private int _improveAmount = 1;
    private int _drawCount = 1;

    private async Task<UkeIreResult> Calculate(object parameter, CancellationToken cancellationToken)
    {
      var hand = _currentHand.Clone();
      if (!hand.IsValid)
      {
        return new UkeIreResult(Enumerable.Empty<UkeIreViewModel>());
      }
      var ukeIreViewModels = await Task.Run(() => GetUkeIreViewModels(hand), cancellationToken);
      return new UkeIreResult(ukeIreViewModels);
    }

    private IEnumerable<UkeIreViewModel> GetUkeIreViewModels(HandCalculator hand)
    {
      var ukeIre = hand.GetDeepUkeIre(DrawCount, ImproveAmount).OrderByDescending(u => u.ImprovementRate);
      return ukeIre.Select(ukeIreInfo => new UkeIreViewModel(ukeIreInfo)).ToList();
    }

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

      _shorthand = _currentHand.ToString();
      UpdateIcons(new ShorthandParser(_shorthand));
      OnPropertyChanged(nameof(Shanten));
      OnPropertyChanged(nameof(Shorthand));
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

      _shorthand = _currentHand.ToString();
      UpdateIcons(new ShorthandParser(_shorthand));
      OnPropertyChanged(nameof(Shanten));
      OnPropertyChanged(nameof(Shorthand));
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

      Calculation.ExecuteAsync(null);
    }

    private void OnRandomize(object obj)
    {
      var count = Convert.ToInt32(obj, CultureInfo.InvariantCulture);

      var rand = new Random((int) DateTime.Now.Ticks);
      var drawn = new bool[136];

      _currentHand = new HandCalculator();
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