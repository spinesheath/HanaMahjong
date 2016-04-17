// Spines.FakeTenhouServer.Server.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Spines.FakeTenhouServer
{
    internal class Server : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Logger _logger;
        private readonly Func<HttpListenerRequest, string> _responder;

        public Server(IEnumerable<string> prefixes, Func<HttpListenerRequest, string> method, Logger logger)
        {
            _logger = logger;

            foreach (var s in prefixes)
            {
                _listener.Prefixes.Add(s);
            }

            if (!_listener.Prefixes.Any())
            {
                throw new ArgumentException("prefixes");
            }

            _responder = method;
            _listener.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        Listen();
                        Thread.Sleep(100);
                    }
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message);
                }
            });
        }

        private void Listen()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var context = (HttpListenerContext) state;
                try
                {
                    Console.WriteLine(context.Request);

                    var response = _responder(context.Request);
                    var buffer = Encoding.UTF8.GetBytes(response);
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message);
                }
                finally
                {
                    context.Response.OutputStream.Close();
                }
            }, _listener.GetContext());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            _listener.Stop();
            _listener.Close();
        }
    }
}