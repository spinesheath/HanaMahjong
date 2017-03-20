// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
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

      InitPlayers();
    }

    public ICollection<PlayerViewModel> Players { get; } = new ObservableCollection<PlayerViewModel>();

    public ICommand Save { get; }

    public ICommand Open { get; }

    public ICommand SaveAs { get; }

    public ICommand New { get; }

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

    private string _file;

    private PlayerViewModel _selectedPlayer;

    private void InitPlayers()
    {
      Players.Add(new PlayerViewModel {Name = "A"});
      Players.Add(new PlayerViewModel {Name = "B"});
      Players.Add(new PlayerViewModel {Name = "C"});
      Players.Add(new PlayerViewModel {Name = "D"});
      SelectedPlayer = Players.First();
    }

    private void OnSaveAs(object obj)
    {
      var root = Serialize();
      if (null == root)
      {
        return;
      }
      try
      {
        using (var dialog = new CommonSaveFileDialog())
        {
          dialog.EnsurePathExists = true;
          dialog.DefaultExtension = "xml";
          dialog.Filters.Add(new CommonFileDialogFilter("xml", "*.xml"));
          dialog.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));
          dialog.DefaultFileName = _file ?? "table.xml";
          var result = dialog.ShowDialog();
          if (result != CommonFileDialogResult.Ok)
          {
            return;
          }
          root.Save(dialog.FileName);
        }
      }
      catch
      {
        MessageBox.Show("Failed to save data.");
      }
    }

    private XElement Serialize()
    {
      try
      {
        var serializer = new DataContractSerializer(typeof(PlayerViewModel));
        var root = new XElement("Table");
        foreach (var player in Players)
        {
          using (var stream = new MemoryStream())
          {
            serializer.WriteObject(stream, player);
            stream.Position = 0;
            using (var reader = XmlReader.Create(stream))
            {
              root.Add(XElement.Load(reader));
            }
          }
        }
        return root;
      }
      catch
      {
        MessageBox.Show("Failed to serialize data.");
      }
      return null;
    }

    private void OnNew(object obj)
    {
      _file = null;
      InitPlayers();
    }

    private void OnOpen(object obj)
    {
      using (var dialog = new CommonOpenFileDialog())
      {
        dialog.EnsureFileExists = true;
        dialog.DefaultExtension = ".xml";
        var result = dialog.ShowDialog();
        if (result != CommonFileDialogResult.Ok)
        {
          return;
        }
        var xel = XElement.Load(dialog.FileName);
      }
    }

    private void OnSave(object obj)
    {
      if (null == _file || !File.Exists(_file))
      {
        OnSaveAs(null);
      }
      // TODO XElement
    }
  }
}