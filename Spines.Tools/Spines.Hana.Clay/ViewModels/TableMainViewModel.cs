﻿// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using Spines.Hana.Clay.Commands;
using Spines.Hana.Clay.Models;
using Spines.Mahjong.Analysis.Classification;
using Spines.Utility;

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
      ExportLatex = new DelegateCommand(OnExportLatex);

      InitPlayers();
    }

    public ObservableCollection<PlayerViewModel> Players { get; } = new ObservableCollection<PlayerViewModel>();

    public ICommand Save { get; }

    public ICommand Open { get; }

    public ICommand SaveAs { get; }

    public ICommand New { get; }

    public ICommand ExportLatex { get; }

    public ICollection<TileViewModel> Tiles { get; } = new ObservableCollection<TileViewModel>();

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

    public string InvalidText
    {
      get { return _invalidText; }
      set
      {
        if (_invalidText == value)
        {
          return;
        }
        _invalidText = value;
        OnPropertyChanged();
      }
    }

    private string _file;

    private PlayerViewModel _selectedPlayer;

    private static readonly Dictionary<Suit, char> SuitCharacters = new Dictionary<Suit, char>
    {
      {Suit.Manzu, 'm'},
      {Suit.Pinzu, 'p'},
      {Suit.Souzu, 's'},
      {Suit.Jihai, 'j'}
    };

    private string _invalidText;

    private void InitPlayers()
    {
      InitPlayers("ABCD".Select(c => new PlayerViewModel(c.ToString())));
    }

    private void InitPlayers(IEnumerable<PlayerViewModel> players)
    {
      foreach (var player in Players)
      {
        player.PropertyChanged -= OnDataChanged;
      }
      Players.Clear();
      foreach (var player in players)
      {
        Players.Add(player);
        player.PropertyChanged += OnDataChanged;
      }
      SelectedPlayer = Players.First();
      UpdateTable();
    }

    private void OnDataChanged(object sender, PropertyChangedEventArgs e)
    {
      Validate();
      UpdateTable();
    }

    private void Validate()
    {
      InvalidText = null;
      var tiles = Players.SelectMany(GetAllTiles);
      var bySuit = tiles.GroupBy(t => t.Suit).ToList();
      if (bySuit.Any(g => g.GroupBy(t => t.Index).Any(gg => gg.Count() > 4)))
      {
        InvalidText = "Too many of the same tile overall.";
      }
      else if (bySuit.Any(g => g.Count(t => t.Aka) > 1))
      {
        InvalidText = "Too many aka dora.";
      }
      else if (Players.SelectMany(p => p.Melds.Where(m => m.Tiles.Count() == 4)).Count() > 4)
      {
        InvalidText = "Too many kans.";
      }
    }

    private void OnExportLatex(object obj)
    {
      try
      {
        var text = CreateLatex();

        using (var dialog = new CommonOpenFileDialog())
        {
          dialog.IsFolderPicker = true;
          dialog.EnsurePathExists = true;
          var result = dialog.ShowDialog();
          if (result != CommonFileDialogResult.Ok)
          {
            return;
          }
          var fileName = Path.Combine(dialog.FileName, "image.tex");
          File.WriteAllText(fileName, text);
        }
      }
      catch
      {
        MessageBox.Show("Failed to export.");
      }
    }

    private string CreateLatex()
    {
      var sb = new StringBuilder();
      sb.AppendLine(@"\documentclass[12pt]{article}");
      sb.AppendLine(@"\usepackage{graphicx}");
      sb.AppendLine(@"\graphicspath{{t_real/}}");
      sb.AppendLine(@"\begin{document}");
      sb.AppendLine(@"\begin{center}");
      sb.AppendLine(@"\begin{picture}(" + TableLayout.HalfTableWidth + "," + TableLayout.HalfTableHeight + ")");
      foreach (var tile in Tiles)
      {
        var fileName = GetFileName(tile);
        var graphic = @"\includegraphics{" + fileName + "}";
        var y = TableLayout.HalfTableHeight - tile.Y;
        if (tile.Tile.Location == TileLocation.Riichi)
        {
          y += 2 * TableLayout.RiichiDistance;
        }
        sb.AppendLine(@"\put(" + tile.X + "," + y + "){" + graphic + "}");
      }
      sb.AppendLine(@"\end{picture}");
      sb.AppendLine(@"\end{center}");
      sb.AppendLine(@"\end{document}");
      return sb.ToString();
    }

    private void UpdateTable()
    {
      var tiles = new LayoutGenerator().Create(Players);
      Tiles.Clear();
      foreach (var tile in tiles)
      {
        Tiles.Add(tile);
      }
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
          _file = dialog.FileName;
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
        var serializer = new DataContractSerializer(typeof(PlayerModel));
        var root = new XElement("Table");
        foreach (var player in Players)
        {
          using (var stream = new MemoryStream())
          {
            serializer.WriteObject(stream, player.GetModel());
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
      try
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

          var root = XElement.Load(dialog.FileName);
          var serializer = new DataContractSerializer(typeof(PlayerModel));
          var players = root.Nodes().Select(node => (PlayerModel) serializer.ReadObject(node.CreateReader())).ToList();
          if (players.Count != 4)
          {
            MessageBox.Show("Invalid number of players.");
            return;
          }

          InitPlayers(players.Select(p => new PlayerViewModel(p)));

          _file = dialog.FileName;
        }
      }
      catch
      {
        MessageBox.Show("Error opening file.");
      }
    }

    private void OnSave(object obj)
    {
      if (null == _file || !File.Exists(_file))
      {
        OnSaveAs(null);
        return;
      }
      var root = Serialize();
      if (null == root)
      {
        return;
      }
      try
      {
        root.Save(_file);
      }
      catch
      {
        MessageBox.Show("Failed to save data.");
      }
    }

    private static IEnumerable<Tile> GetAllTiles(PlayerViewModel p)
    {
      var pond = p.Pond.Where(t => !t.IsGhost);
      var melded = p.Melds.SelectMany(m => m.Tiles);
      var draw = p.Draw.HasValue ? p.Draw.Value.Yield() : Enumerable.Empty<Tile>();
      return p.Tiles.Concat(pond).Concat(melded).Concat(draw);
    }

    private static string GetFileName(TileViewModel tileViewModel)
    {
      var tile = tileViewModel.Tile;
      var location = tile.Location;
      var suitCharacter = SuitCharacters[tile.Suit];
      var tileNumber = tile.Aka ? "e" : (tile.Index + 1).ToString();
      switch (location)
      {
        case TileLocation.Concealed:
          return $"0{suitCharacter}{tileNumber}.png";
        case TileLocation.Discarded:
        case TileLocation.Melded:
          return $"1{suitCharacter}{tileNumber}.png";
        case TileLocation.Added:
        case TileLocation.Called:
        case TileLocation.Riichi:
          return $"2{suitCharacter}{tileNumber}.png";
        case TileLocation.FaceDown:
          return "1j9.png";
        default:
          return "1j9.png";
      }
    }
  }
}