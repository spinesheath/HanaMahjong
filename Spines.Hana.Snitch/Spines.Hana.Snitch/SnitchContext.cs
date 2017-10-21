﻿// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spines.Hana.Snitch.Properties;

namespace Spines.Hana.Snitch
{
  internal class SnitchContext : ApplicationContext
  {
    public SnitchContext()
    {
      _icon = new NotifyIcon
      {
        Icon = Resources.AppIcon,
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

      UpdateMenu();
    }

    public void OnExit(object sender, EventArgs e)
    {
      _icon.Visible = false;
    }

    private const string HanablameUrl = "http://www.hanablame.com";
    private static readonly Regex ConfigIniRegex = new Regex(@"^\d+=file=(\d{10}gm-\d{4}-\d{4}-[\da-f]{8}).*sc=");
    //private static readonly RegistryKey AutostartRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
    private readonly NotifyIcon _icon;
    private string _balloonUrl = HanablameUrl;
    private static readonly ConcurrentQueue<DateTime> FileChangeQueue = new ConcurrentQueue<DateTime>();
    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
    private static readonly HttpClient Client = new HttpClient();

    private void UpdateMenu()
    {
      _icon.ContextMenu = BuildMenu();
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

    private async void OnConfigIniChanged(object sender, FileSystemEventArgs e)
    {
      await OnConfigIniChanged(e.FullPath);
    }

    /// <summary>
    /// Queues a timestamp, then waits until a second after the last timestamp.
    /// FileSystemWatcher sometimes raises multiple events for a single modification.
    /// </summary>
    private async Task OnConfigIniChanged(string path)
    {
      FileChangeQueue.Enqueue(DateTime.UtcNow);

      if (Semaphore.CurrentCount == 0)
      {
        return;
      }
      await Semaphore.WaitAsync();
      try
      {
        while (FileChangeQueue.TryDequeue(out var next))
        {
          var target = next + TimeSpan.FromSeconds(1);
          var now = DateTime.UtcNow;
          if (target > now)
          {
            await Task.Delay(target - now);
          }
        }

        try
        {
          var newIds = ReadConfigIni(path);
          foreach (var id in newIds)
          {
            await Task.Delay(TimeSpan.FromSeconds(5));
            await Client.GetStringAsync(GetSnitchUrl(id));
            _balloonUrl = GetReviewUrl(id);
            ShowBalloon("Snitched!", "Click to review on hanablame.com");
          }
        }
        catch (IOException)
        {
          await OnConfigIniChanged(path);
          Logger.Warn("IOException on file change");
        }
      }
      catch (Exception ex)
      {
        ShowError(ex);
        Logger.Error(ex, "on file change");
      }
      finally
      {
        Semaphore.Release();
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

    private void OnBalloonClicked(object sender, EventArgs e)
    {
      OpenUrl(_balloonUrl);
    }

    private IEnumerable<string> ReadConfigIni(string path)
    {
      var lines = File.ReadAllLines(path);
      var matches = lines.Select(l => ConfigIniRegex.Match(l)).Where(m => m.Success);
      var ids = matches.Select(m => m.Groups[1].Value).ToList();
      ids.Reverse();

      var recent = new HashSet<string>(History.All());
      var newIds = ids.Where(id => !recent.Contains(id)).ToList();
      if (!newIds.Any())
      {
        return Enumerable.Empty<string>();
      }

      History.Append(newIds);

      UpdateMenu();

      return newIds;
    }

    private static ContextMenu BuildMenu()
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

    private static string GetReviewUrl(string id)
    {
      return $"{HanablameUrl}/?r={id}";
    }

    private static void OpenBlame(object sender, EventArgs e)
    {
      var item = (MenuItem) sender;
      OpenUrl(GetReviewUrl(item.Text));
    }

    private static string GetSnitchUrl(string id)
    {
      return $"{HanablameUrl}/api/snitch/?replayId={id}";
    }

    private static void OpenUrl(string url)
    {
      try
      {
        Process.Start(url);
      }
      catch (Exception ex)
      {
        Logger.Error(ex, $"open {url}");
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