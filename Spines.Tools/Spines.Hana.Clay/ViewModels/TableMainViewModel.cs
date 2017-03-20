// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spines.Hana.Clay.ViewModels
{
  internal class TableMainViewModel : ViewModelBase
  {
    public TableMainViewModel()
    {
      Players.Add(new PlayerViewModel("A"));
      Players.Add(new PlayerViewModel("B"));
      Players.Add(new PlayerViewModel("C"));
      Players.Add(new PlayerViewModel("D"));
    }

    public ICollection<PlayerViewModel> Players { get; } = new ObservableCollection<PlayerViewModel>();

    public PlayerViewModel SelectedPlayer
    {
      get { return _selectedPlayer; }
      set
      {
        if (_selectedPlayer == value)
        {
          return;
        }
        _selectedPlayer = value;
        OnPropertyChanged();
      }
    }

    private PlayerViewModel _selectedPlayer;
  }
}