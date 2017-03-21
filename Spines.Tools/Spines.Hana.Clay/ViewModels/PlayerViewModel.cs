// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Hana.Clay.ViewModels
{
  [DataContract]
  internal class PlayerViewModel : ViewModelBase
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
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

    public ICollection<Tile> Tiles { get; } = new ObservableCollection<Tile>();

    public ICollection<Meld> Melds { get; } = new ObservableCollection<Meld>();

    public Tile? Draw
    {
      get { return _draw; }
      set
      {
        _draw = value;
        OnPropertyChanged();
      }
    }

    private Tile? _draw;

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

        HandIsValid = true;
      }
      catch
      {
        HandIsValid = false;
      }
    }

    public bool HandIsValid
    {
      get { return _handIsValid; }
      set
      {
        if (_handIsValid == value)
        {
          return;
        }
        _handIsValid = value;
        OnPropertyChanged();
      }
    }

    private bool _handIsValid;

    [DataMember]
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
        OnPropertyChanged();
      }
    }

    [DataMember]
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

    [DataMember]
    public bool Riichi
    {
      get { return _riichi; }
      set
      {
        if (_riichi == value)
        {
          return;
        }
        _riichi = value;
        OnPropertyChanged();
      }
    }

    private bool _riichi;
    private string _handShorthand;
    private string _pondShorthand;
    private string _score;
  }
}