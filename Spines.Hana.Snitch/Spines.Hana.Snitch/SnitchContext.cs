// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Spines.Hana.Snitch.Properties;

namespace Spines.Hana.Snitch
{
  // TODO access leveldb in chrome. Also access localstorage in chrome.
  // TODO access localstorage in firefox
  internal class SnitchContext : ApplicationContext
  {
    public SnitchContext()
    {
      _icon = new NotifyIcon
      {
        Icon = Resources.AppIcon,
        Visible = true
      };

      _watchers.Add(new PremiumWatcher(Handler));
      _watchers.Add(new FirefoxFlashWatcher(Handler));
      _watchers.Add(new ChromeFlashWatcher(Handler));

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
    private readonly List<Watcher> _watchers = new List<Watcher>();
    private static readonly RegistryKey AutostartRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
    private readonly NotifyIcon _icon;
    private string _balloonUrl = HanablameUrl;
    private static readonly HttpClient Client = new HttpClient();

    private async Task Handler(IEnumerable<ReplayData> newReplays)
    {
      foreach (var replayData in newReplays)
      {
        await Task.Delay(TimeSpan.FromSeconds(5));
        var response = await Client.GetAsync(GetSnitchUrl(replayData));
        if (response.IsSuccessStatusCode)
        {
          History.Success(replayData);
          UpdateMenu();
          _balloonUrl = GetReviewUrl(replayData);
          ShowBalloon("Snitched!", "Click to review on hanablame.com");
        }
        else
        {
          Logger.Warn($"Failed to snitch {replayData.Id}.");
          History.Fail(replayData);
        }
      }
    }

    private void UpdateMenu()
    {
      _icon.ContextMenu = BuildMenu();
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

    private ContextMenu BuildMenu()
    {
      var menu = new ContextMenu();

      foreach (var replay in History.Recent(10))
      {
        menu.MenuItems.Add(new MenuItem(replay.Id, OpenBlame) {Tag = replay});
      }
      if (menu.MenuItems.Count > 0)
      {
        menu.MenuItems.Add("-");
      }

      menu.MenuItems.Add(new MenuItem("Scan", OnScan));
      menu.MenuItems.Add("-");
      TryAddAutostart(menu);
      AddDisableNotifications(menu);
      menu.MenuItems.Add(new MenuItem("Open error log", OpenErrorLog));
      menu.MenuItems.Add("-");
      menu.MenuItems.Add(new MenuItem("Exit", Exit));
      return menu;
    }

    private async void OnScan(object sender, EventArgs e)
    {
      var tasks = _watchers.Select(w => w.Scan());
      await Task.WhenAll(tasks);
    }

    private static void OpenErrorLog(object sender, EventArgs e)
    {
      Logger.OpenLogFile();
    }

    private static void OpenBlame(object sender, EventArgs e)
    {
      var item = (MenuItem) sender;
      var replayData = (ReplayData) item.Tag;
      OpenUrl(GetReviewUrl(replayData));
    }

    private static string GetReviewUrl(ReplayData replayData)
    {
      return $"{HanablameUrl}/?f=0&g=0&p={replayData.Position}&r={replayData.Id}";
    }

    private static string GetSnitchUrl(ReplayData replayData)
    {
      return $"{HanablameUrl}/api/snitch/?replayId={replayData.Id}";
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
      if (AutostartRegistryKey == null)
      {
        return;
      }
      var item = new MenuItem("Autostart");
      var value = AutostartRegistryKey.GetValue(Application.ProductName) as string;
      item.Checked = value == Application.ExecutablePath;
      item.Click += OnAutostartChanged;
      menu.MenuItems.Add(item);
    }

    private static void OnAutostartChanged(object sender, EventArgs e)
    {
      var item = (MenuItem) sender;
      item.Checked = !item.Checked;
      if (item.Checked)
      {
        AutostartRegistryKey.SetValue(Application.ProductName, Application.ExecutablePath);
      }
      else
      {
        AutostartRegistryKey.DeleteValue(Application.ProductName, false);
      }
    }
  }
}