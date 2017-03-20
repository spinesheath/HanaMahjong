// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Spines.Hana.Clay.Commands;

namespace Spines.Hana.Clay.ViewModels
{
  internal class TableMainViewModel : ViewModelBase
  {
    public TableMainViewModel()
    {
      Save = new DelegateCommand(OnSave);
      Open = new DelegateCommand(OnOpen);
      New = new DelegateCommand(OnNew);
      SaveAs = new DelegateCommand(OnSaveAs);

      Players.Add(new PlayerViewModel("A"));
      Players.Add(new PlayerViewModel("B"));
      Players.Add(new PlayerViewModel("C"));
      Players.Add(new PlayerViewModel("D"));
    }

    private void OnSaveAs(object obj)
    {
      
    }

    private void OnNew(object obj)
    {
      
    }

    private void OnOpen(object obj)
    {
      
    }

    private void OnSave(object obj)
    {
      
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

    public ICommand Save { get; }

    public ICommand Open { get; }

    public ICommand SaveAs { get; }

    public ICommand New { get; }

    private PlayerViewModel _selectedPlayer;
  }
}