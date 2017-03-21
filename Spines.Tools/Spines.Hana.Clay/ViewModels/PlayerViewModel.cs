// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spines.Hana.Clay.Models;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.ViewModels
{
  internal class PlayerViewModel : ViewModelBase
  {
    public PlayerViewModel(string name)
    {
      Name = name;
    }

    public PlayerViewModel(PlayerModel model)
      : this(model.Name)
    {
      HandShorthand = model.HandShorthand;
      PondShorthand = model.PondShorthand;
      Score = model.Score;
    }

    public string Name { get; }

    public ICollection<Tile> Tiles { get; } = new ObservableCollection<Tile>();

    public ICollection<Tile> Pond { get; } = new ObservableCollection<Tile>();

    public ICollection<Meld> Melds { get; } = new ObservableCollection<Meld>();

    public string HandShorthand
    {
      get { return _handShorthand; }
      set
      {
        if (_handShorthand == value)
        {
          return;
        }
        _handShorthand = value;
        UpdateHand();
        OnPropertyChanged();
      }
    }

    public Tile? Draw
    {
      get { return _draw; }
      set
      {
        _draw = value;
        OnPropertyChanged();
      }
    }

    public string PondError
    {
      get { return _pondError; }
      set
      {
        if (_pondError == value)
        {
          return;
        }
        _pondError = value;
        OnPropertyChanged();
      }
    }

    public string HandError
    {
      get { return _handError; }
      set
      {
        if (_handError == value)
        {
          return;
        }
        _handError = value;
        OnPropertyChanged();
      }
    }

    public string PondShorthand
    {
      get { return _pondShorthand; }
      set
      {
        if (_pondShorthand == value)
        {
          return;
        }
        _pondShorthand = value;
        UpdatePond();
        OnPropertyChanged();
      }
    }

    public string Score
    {
      get { return _score; }
      set
      {
        if (_score == value)
        {
          return;
        }
        _score = value;
        OnPropertyChanged();
      }
    }

    public PlayerModel GetModel()
    {
      return new PlayerModel {Name = Name, HandShorthand = HandShorthand, PondShorthand = PondShorthand, Score = Score};
    }

    private string _pondError;
    private Tile? _draw;
    private string _handError;
    private string _handShorthand;
    private string _pondShorthand;
    private string _score;

    private void UpdatePond()
    {
      try
      {
        var pond = PondParser.Parse(PondShorthand);
        Pond.Clear();
        foreach (var tile in pond)
        {
          Pond.Add(tile);
        }

        PondError = null;
      }
      catch (FormatException e)
      {
        PondError = e.Message;
      }
    }

    private void UpdateHand()
    {
      try
      {
        var hand = HandParser.Parse(HandShorthand);
        Tiles.Clear();
        foreach (var tile in hand.Tiles)
        {
          Tiles.Add(tile);
        }
        Melds.Clear();
        foreach (var meld in hand.Melds)
        {
          Melds.Add(meld);
        }
        Draw = hand.Draw;

        HandError = null;
      }
      catch (FormatException e)
      {
        HandError = e.Message;
      }
    }
  }
}