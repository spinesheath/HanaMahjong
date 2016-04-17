// Spines.FakeTenhouServer.Logger.cs
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

using System;
using System.IO;

namespace Spines.FakeTenhouServer
{
    internal class Logger
    {
        private readonly string _logDirectory;
        private readonly string _logFilePath;

        public Logger(string applicationName, int lineCount)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _logDirectory = Path.Combine(appData, applicationName);
            _logFilePath = Path.Combine(_logDirectory, "log.txt");
            TrimLog(lineCount);
        }

        private void TrimLog(int lineCount)
        {
            lock (this)
            {
                if (!File.Exists(_logFilePath))
                {
                    return;
                }
                var lines = File.ReadAllLines(_logFilePath);
                var toWrite = lines.TakeLast(lineCount);
                File.WriteAllLines(_logFilePath, toWrite);
            }
        }

        public void Log(string message)
        {
            CreateDirectory();
            lock (this)
            {
                File.AppendAllLines(_logFilePath, message.Yield());
            }
        }

        private void CreateDirectory()
        {
            lock (this)
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
            }
        }
    }
}