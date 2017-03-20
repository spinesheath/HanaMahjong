// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Hana.Clay.ViewModels
{
  [Serializable]
  internal class PlayerViewModel : ViewModelBase
  {
    public PlayerViewModel(string name)
    {
      Name = name;
    }

    public string Name { get; }

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