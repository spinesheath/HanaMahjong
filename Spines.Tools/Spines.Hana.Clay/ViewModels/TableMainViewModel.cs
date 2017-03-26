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

    private string _file;

    private PlayerViewModel _selectedPlayer;

    private static readonly Dictionary<Suit, char> SuitCharacters = new Dictionary<Suit, char>
    {
      {Suit.Manzu, 'm'},
      {Suit.Pinzu, 'p'},
      {Suit.Souzu, 's'},
      {Suit.Jihai, 'j'}
    };

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
      UpdateTable();
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
      Tiles.Clear();
      AddPlayer1Tiles();
      AddPlayer2Tiles();
      AddPlayer3Tiles();
      AddPlayer4Tiles();
      var tiles = Tiles.OrderBy(t => t.Y).ToList();
      foreach (var tile in tiles)
      {
        Tiles.Add(tile);
      }
    }

    private void AddPlayer4Tiles()
    {
      const int playerId = 3;
      var p = Players[playerId];
      var kanShift = GetKanShift(p);
      var y = TableLayout.HalfTableWidth - 3 * TableLayout.TileWidth - 3 * TableLayout.TileHeight - TableLayout.VerticalTileThickness - TableLayout.HorizontalPondToHand;
      var x = TableLayout.HalfTableHeight - 7 * TableLayout.TileWidth - TableLayout.VerticalTileThickness - kanShift;
      AddConcealedTiles(p, x, y, playerId);

      y = TableLayout.HalfTableWidth - 3 * TableLayout.TileWidth - 4 * TableLayout.TileHeight - TableLayout.HorizontalPondToHand;
      x = TableLayout.HalfTableHeight + 3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.TileHeight + TableLayout.HorizontalTileThickness + TableLayout.VerticalPondToHand;
      foreach (var meld in p.Melds.Reverse())
      {
        foreach (var tile in meld.Tiles.Reverse())
        {
          if (tile.Location == TileLocation.Added)
          {
            Tiles.Add(CreateTileWithXySwitch(tile, x - TableLayout.TileHeight, y + TableLayout.TileWidth, playerId));
          }
          else
          {
            x -= tile.Location == TileLocation.Called ? TableLayout.TileHeight : TableLayout.TileWidth;
            Tiles.Add(CreateTileWithXySwitch(tile, x, y, playerId));
          }
        }
        x -= TableLayout.MeldDistance;
      }

      y = TableLayout.HalfTableWidth - 3 * TableLayout.TileWidth - TableLayout.TileHeight;
      x = TableLayout.HalfTableHeight - 3 * TableLayout.TileWidth;
      AddPond(p, x, y, playerId);
    }

    private void AddPlayer3Tiles()
    {
      const int playerId = 2;
      var p = Players[playerId];
      var kanShift = GetKanShift(p);
      var x = TableLayout.HalfTableWidth + 6 * TableLayout.TileWidth + kanShift;
      var y = TableLayout.HalfTableHeight - 3 * TableLayout.TileWidth - 4 * TableLayout.TileHeight - TableLayout.VerticalPondToHand - TableLayout.HorizontalTileThickness;
      AddConcealedTiles(p, x, y, playerId);

      x = TableLayout.HalfTableWidth - (3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.HorizontalPondToHand + TableLayout.TileHeight);
      foreach (var meld in p.Melds.Reverse())
      {
        foreach (var tile in meld.Tiles.Reverse())
        {
          if (tile.Location == TileLocation.Added)
          {
            x += TableLayout.TileHeight;
            Tiles.Add(CreateTileWithXySwitch(tile, x, y - TableLayout.TileHeight - TableLayout.TileWidth - TableLayout.TileWidth, playerId));
            x -= TableLayout.TileHeight;
          }
          else
          {
            Tiles.Add(CreateTileWithXySwitch(tile, x, y, playerId));
            x += tile.Location == TileLocation.Called ? TableLayout.TileHeight : TableLayout.TileWidth;
          }
        }
        x += TableLayout.MeldDistance;
      }

      x = TableLayout.HalfTableWidth + 2 * TableLayout.TileWidth;
      y = TableLayout.HalfTableHeight - 3 * TableLayout.TileWidth - TableLayout.TileHeight;
      AddPond(p, x, y, playerId);
    }

    private void AddPlayer2Tiles()
    {
      const int playerId = 1;
      var p = Players[playerId];
      var kanShift = GetKanShift(p);
      var y = TableLayout.HalfTableWidth + 3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.HorizontalPondToHand;
      var x = TableLayout.HalfTableHeight + 6 * TableLayout.TileWidth + kanShift;
      AddConcealedTiles(p, x, y, playerId);

      x = TableLayout.HalfTableHeight - (3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.TileHeight + TableLayout.HorizontalTileThickness + TableLayout.VerticalPondToHand);
      foreach (var meld in p.Melds.Reverse())
      {
        foreach (var tile in meld.Tiles.Reverse())
        {
          if (tile.Location == TileLocation.Added)
          {
            Tiles.Add(CreateTileWithXySwitch(tile, x, y + TableLayout.TileHeight - TableLayout.TileWidth - TableLayout.TileWidth, playerId));
          }
          else
          {
            var h = tile.Location == TileLocation.Called ? TableLayout.TileHeight - TableLayout.TileWidth : 0;
            Tiles.Add(CreateTileWithXySwitch(tile, x, y + h, playerId));
            x += tile.Location == TileLocation.Called ? TableLayout.TileHeight : TableLayout.TileWidth;
          }
        }
        x += TableLayout.MeldDistance;
      }

      y = TableLayout.HalfTableWidth + 3 * TableLayout.TileWidth;
      x = TableLayout.HalfTableHeight + 2 * TableLayout.TileWidth;
      AddPond(p, x, y, playerId);
    }

    private void AddPlayer1Tiles()
    {
      const int playerId = 0;
      var p = Players[playerId];
      var kanShift = GetKanShift(p);
      var x = TableLayout.HalfTableWidth - 7 * TableLayout.TileWidth - kanShift;
      var y = TableLayout.HalfTableHeight + 3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.HorizontalTileThickness + TableLayout.VerticalPondToHand;
      AddConcealedTiles(p, x, y, playerId);

      x = TableLayout.HalfTableWidth + 3 * TableLayout.TileWidth + 3 * TableLayout.TileHeight + TableLayout.HorizontalPondToHand + TableLayout.TileHeight;
      foreach (var meld in p.Melds.Reverse())
      {
        foreach (var tile in meld.Tiles.Reverse())
        {
          if (tile.Location == TileLocation.Added)
          {
            x -= TableLayout.TileHeight;
            Tiles.Add(CreateTileWithXySwitch(tile, x, y + TableLayout.TileHeight - TableLayout.TileWidth - TableLayout.TileWidth, playerId));
            x += TableLayout.TileHeight;
          }
          else
          {
            var h = tile.Location == TileLocation.Called ? TableLayout.TileHeight - TableLayout.TileWidth : 0;
            x -= tile.Location == TileLocation.Called ? TableLayout.TileHeight : TableLayout.TileWidth;
            Tiles.Add(CreateTileWithXySwitch(tile, x, y + h, playerId));
          }
        }
        x -= TableLayout.MeldDistance;
      }

      x = TableLayout.HalfTableWidth - 3 * TableLayout.TileWidth;
      y = TableLayout.HalfTableHeight + 3 * TableLayout.TileWidth;
      AddPond(p, x, y, playerId);
    }

    private void AddPond(PlayerViewModel p, int x, int y, int playerId)
    {
      var factorX = playerId == 0 || playerId == 3 ? 1 : -1;
      var factorY = playerId < 2 ? 1 : -1;
      var riichiX = playerId == 0 || playerId == 3 ? 0 : TableLayout.TileHeight - TableLayout.TileWidth;
      var riichiY = playerId < 2 ? TableLayout.RiichiDistance : TableLayout.TileWidth - TableLayout.TileHeight + TableLayout.RiichiDistance;

      var startX = x;
      var pondColumn = 0;
      var pondRow = 0;
      foreach (var tile in p.Pond)
      {
        if (tile.Location == TileLocation.Riichi)
        {
          Tiles.Add(CreateTileWithXySwitch(tile, x - riichiX, y + riichiY, playerId));
          x += TableLayout.TileHeight * factorX;
        }
        else
        {
          Tiles.Add(CreateTileWithXySwitch(tile, x, y, playerId));
          x += TableLayout.TileWidth * factorX;
        }
        pondColumn += 1;
        if (pondColumn == 6 && pondRow != 2)
        {
          pondRow += 1;
          pondColumn = 0;
          x = startX;
          y += TableLayout.TileHeight * factorY;
        }
      }
    }

    private void AddConcealedTiles(PlayerViewModel p, int x, int y, int playerId)
    {
      var factor = playerId == 0 || playerId == 3 ? 1 : -1;
      foreach (var tile in p.Tiles)
      {
        Tiles.Add(CreateTileWithXySwitch(tile, x, y, playerId));
        x += TableLayout.TileWidth * factor;
      }
      if (p.Draw.HasValue)
      {
        x += TableLayout.DrawDistance * factor;
        Tiles.Add(CreateTileWithXySwitch(p.Draw.Value, x, y, playerId));
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

    private static TileViewModel CreateTileWithXySwitch(Tile tile, int x, int y, int playerId)
    {
      return playerId % 2 == 0 ? new TileViewModel(tile, x, y, playerId) : new TileViewModel(tile, y, x, playerId);
    }

    private static int GetKanShift(PlayerViewModel player)
    {
      var wideKanCount = player.Melds.Count(m => m.Tiles.Count() == 4 && m.Tiles.All(t => t.Location != TileLocation.Added));
      return wideKanCount < 4 ? 0 : TableLayout.TileWidth;
    }
  }
}