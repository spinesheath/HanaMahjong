// Spines.FakeTenhouServer.MainWindow.xaml.cs
// 
// Copyright (C) 2016  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using Spines.FakeTenhouServer.Annotations;

namespace Spines.FakeTenhouServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        //private const string HostFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        private const string HostFilePath = @"D:\temp\hosts";
        private const string RedirectLine = "\t127.0.0.1\th.mjv.jp # redirect tenhou to localhost";
        private readonly Logger _logger;
        private readonly Server _server;
        private const string PrefixLocalhost = "http://localhost:8080/";

        public MainWindow()
        {
            _logger = new Logger("FakeTenhouServer", 100);
            _logger.Log("Starting");

            _server = StartServer();

            UpdateStatus();

            InitializeComponent();
        }

        private Server StartServer()
        {
            var server = new Server(PrefixLocalhost.Yield().ToArray(), Listener, _logger);
            server.Run();
            return server;
        }

        private static string Listener(HttpListenerRequest arg)
        {
            return string.Empty;
        }

        public bool CanDisable { get; private set; }

        public bool CanEnable { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateStatus()
        {
            CanDisable = IsRedirectionActive();
            CanEnable = !CanDisable;
            OnPropertyChanged(nameof(CanDisable));
            OnPropertyChanged(nameof(CanEnable));
        }

        private void OnEnableRedirection(object sender, RoutedEventArgs e)
        {
            EnableRedirection();
        }

        private void EnableRedirection()
        {
            if (IsRedirectionActive())
            {
                return;
            }
            _logger.Log("Enabling redirection");
            File.AppendAllLines(HostFilePath, RedirectLine.Yield());
            UpdateStatus();
        }

        private bool IsRedirectionActive()
        {
            _logger.Log("Checking redirection status");
            var text = File.ReadAllLines(HostFilePath);
            return text.Any(line => line == RedirectLine);
        }

        private void OnDisableRedirection(object sender, RoutedEventArgs e)
        {
            DisableRedirection();
        }

        private void DisableRedirection()
        {
            if (!IsRedirectionActive())
            {
                return;
            }
            _logger.Log("Disabling redirection");
            var lines = File.ReadAllLines(HostFilePath);
            var exceptRedirect = lines.Where(line => line != RedirectLine);
            File.WriteAllLines(HostFilePath, exceptRedirect);
            UpdateStatus();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _server.Dispose();
            _logger.Log("Closing");
            DisableRedirection();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}