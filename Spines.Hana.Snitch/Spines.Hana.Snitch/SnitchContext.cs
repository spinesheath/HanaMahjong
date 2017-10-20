// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Spines.Hana.Snitch.Properties;

namespace Spines.Hana.Snitch
{
  internal class SnitchContext : ApplicationContext
  {
    public SnitchContext()
    {
      var menu = BuildMenu();

      _icon = new NotifyIcon
      {
        Icon = Resources.AppIcon,
        ContextMenu = menu,
        Visible = true
      };

      WatchWindowsClient();

      _icon.BalloonTipClicked += OnBalloonClicked;

      if (!Settings.Default.ShowedFirstLaunchInfo)
      {
        const string title = "Hana Snitch";
        const string body = "Enable or disable autorun in the context menu.\r\nClick for details.";
        ShowBalloon(title, body);
        Settings.Default.ShowedFirstLaunchInfo = true;
        Settings.Default.Save();
      }
    }

    public void OnExit(object sender, EventArgs e)
    {
      _icon.Visible = false;
    }

    private static readonly Regex ConfigIniRegex = new Regex(@"^\d+=file=(\d{10}gm-\d{4}-\d{4}-[\da-f]{8})");
    //private static readonly RegistryKey AutostartRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
    private readonly NotifyIcon _icon;

    private DateTime _lastChange = DateTime.MinValue;

    private ContextMenu BuildMenu()
    {
      var menu = new ContextMenu();

      // the last couple replays as a list
      foreach (var id in History.Recent(10))
      {
        menu.MenuItems.Add(new MenuItem(id, OpenBlame));
      }
      if (menu.MenuItems.Count > 0)
      {
        menu.MenuItems.Add("-");
      }

      TryAddAutostart(menu);
      AddDisableNotifications(menu);
      menu.MenuItems.Add("-");
      menu.MenuItems.Add(new MenuItem("Exit", Exit));
      return menu;
    }

    private void OpenBlame(object sender, EventArgs e)
    {
      var item = (MenuItem) sender;
      OpenBlame(item.Text);
    }

    private void OpenBlame(string id)
    {
      ShowBalloon("Look!", id);
      //System.Diagnostics.Process.Start(hanablame.com/?r={id} p={?});
    }

    private void WatchWindowsClient()
    {
      var roaming = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      var folder = Path.Combine(roaming, "C-EGG", "tenhou", "130");
      if (!Directory.Exists(folder))
      {
        return;
      }
      var fsw = new FileSystemWatcher(folder, "config.ini");
      fsw.NotifyFilter = NotifyFilters.LastWrite;
      fsw.Changed += OnConfigIniChanged;
      fsw.EnableRaisingEvents = true;
    }

    private void OnConfigIniChanged(object sender, FileSystemEventArgs e)
    {
      try
      {
        // FileSystemWatcher sometimes raises multiple events for a single modification.
        var now = DateTime.UtcNow;
        var isNewChange = now - _lastChange > TimeSpan.FromSeconds(10);
        _lastChange = now;

        if (isNewChange)
        {
          var newIds = ReadConfigIni(e);

          var body = string.Join(Environment.NewLine, newIds);
          ShowBalloon("New:", body);
        }
      }
      catch (Exception exception)
      {
        ShowError(exception);
      }
    }

    private void ShowError(Exception exception)
    {
      if (Settings.Default.ShowNotifications)
      {
        _icon.ShowBalloonTip(30000, "Error", exception.Message, ToolTipIcon.Error);
      }
    }

    private void ShowBalloon(string title, string body)
    {
      if (Settings.Default.ShowNotifications)
      {
        _icon.ShowBalloonTip(30000, title, body, ToolTipIcon.None);
      }
    }

    private static void OnBalloonClicked(object sender, EventArgs e)
    {
      try
      {
        Process.Start("http://www.hanablame.com");
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
        throw;
      }
    }

    private static void AddDisableNotifications(Menu menu)
    {
      var item = new MenuItem("Show Notifications", OnChangeNotifications);
      item.Checked = Settings.Default.ShowNotifications;
      menu.MenuItems.Add(item);
    }

    private static void OnChangeNotifications(object sender, EventArgs e)
    {
      var item = (MenuItem) sender;
      item.Checked = !item.Checked;
      Settings.Default.ShowNotifications = item.Checked;
      Settings.Default.Save();
    }

    private static void Exit(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private static IEnumerable<string> ReadConfigIni(FileSystemEventArgs e)
    {
        var lines = File.ReadAllLines(e.FullPath);
        var matches = lines.Select(l => ConfigIniRegex.Match(l)).Where(m => m.Success);
        var ids = matches.Select(m => m.Groups[1].Value);

        var recent = History.All();
        var newIds = ids.Except(recent).ToList();
        History.Append(newIds);

        return newIds;
    }

    private static void TryAddAutostart(Menu menu)
    {
      //if (AutostartRegistryKey == null)
      //{
      //  return;
      //}
      //var item = new MenuItem("Autostart");
      //var value = AutostartRegistryKey.GetValue(Application.ProductName) as string;
      //item.Checked = value == Application.ExecutablePath;
      //item.Click += OnAutostartChanged;
      //menu.MenuItems.Add(item);
    }

    private static void OnAutostartChanged(object sender, EventArgs e)
    {
      //var item = (MenuItem) sender;
      //item.Checked = !item.Checked;
      //if (item.Checked)
      //{
      //  AutostartRegistryKey.SetValue(Application.ProductName, Application.ExecutablePath);
      //}
      //else
      //{
      //  AutostartRegistryKey.DeleteValue(Application.ProductName, false);
      //}
    }

    private static void WatchFlashClient()
    {
      var roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      var shared = Path.Combine(roaming, "Macromedia", "Flash Player", "#SharedObjects");
      if (!Directory.Exists(shared))
      {
        return;
      }
      var mjv = Directory.GetDirectories(shared, "mjv.jp", SearchOption.AllDirectories).FirstOrDefault();
      if (mjv == null)
      {
        return;
      }
      var fsw = new FileSystemWatcher(mjv, "mjinfo.sol");
      fsw.NotifyFilter = NotifyFilters.LastWrite;
      fsw.Changed += OnMjinfoSolChanged;
      fsw.EnableRaisingEvents = true;
    }

    private static void OnMjinfoSolChanged(object sender, FileSystemEventArgs e)
    {
    }
  }
}