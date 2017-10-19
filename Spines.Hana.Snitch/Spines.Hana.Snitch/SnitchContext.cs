// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using Spines.Hana.Snitch.Properties;

namespace Spines.Hana.Snitch
{
  internal class SnitchContext : ApplicationContext
  {
    // TODO recent files in UserSettings
    // TODO Info Bubble
    // TODO Hyperlink

    public SnitchContext()
    {
      _autostartMenuItem = TryCreateAutostartMenuItem();

      var menu = BuildMenu();

      _icon = new NotifyIcon
      {
        Icon = Resources.AppIcon,
        ContextMenu = menu,
        Visible = true
      };

      WatchWindowsClient();
    }

    public void OnExit(object sender, EventArgs e)
    {
      _icon.Visible = false;
    }

    private static readonly HttpClient Client = new HttpClient();
    private static readonly Regex ConfigIniRegex = new Regex(@"^\d+=file=(\d{10}gm-\d{4}-\d{4}-[\da-f]{8})");
    private static readonly RegistryKey AutostartRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
    private readonly NotifyIcon _icon;
    private readonly MenuItem _autostartMenuItem;

    private ContextMenu BuildMenu()
    {
      var menu = new ContextMenu();
      // the last 5 replays as a list
      menu.MenuItems.Add(new MenuItem("Recent"));
      menu.MenuItems.Add("-");
      if (_autostartMenuItem != null)
      {
        menu.MenuItems.Add(_autostartMenuItem);
        menu.MenuItems.Add("-");
      }
      menu.MenuItems.Add(new MenuItem("Exit", Exit));
      return menu;
    }

    /// <summary>
    /// Creates the Autostart MenuItem if the autostart registry key is found.
    /// </summary>
    private MenuItem TryCreateAutostartMenuItem()
    {
      if (AutostartRegistryKey == null)
      {
        return null;
      }
      var item = new MenuItem("Autostart");
      var value = AutostartRegistryKey.GetValue(Application.ProductName) as string;
      item.Checked = value == Application.ExecutablePath;
      item.Click += OnAutostartChanged;
      return item;
    }

    private void OnAutostartChanged(object sender, EventArgs e)
    {
      _autostartMenuItem.Checked = !_autostartMenuItem.Checked;
      if (_autostartMenuItem.Checked)
      {
        AutostartRegistryKey.SetValue(Application.ProductName, Application.ExecutablePath);
      }
      else
      {
        AutostartRegistryKey.DeleteValue(Application.ProductName, false);
      }
    }

    private static void Exit(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private static void WatchWindowsClient()
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

    private static void OnConfigIniChanged(object sender, FileSystemEventArgs e)
    {
      var lines = File.ReadAllLines(e.FullPath);
      var matches = lines.Select(l => ConfigIniRegex.Match(l)).Where(m => m.Success);
      var ids = matches.Select(m => m.Groups[1].Value).ToList();
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