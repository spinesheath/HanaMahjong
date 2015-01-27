// Spines.Tenhou.ConsoleClient.Program.cs
// 
// Copyright (C) 2014  Johannes Heckl
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
using Spines.Tenhou.Client;

namespace Spines.Tenhou.ConsoleClient
{
  internal class Program
  {
    private static void Main()
    {
      //var l = new ConsoleLogger();
      //using (var t = new TenhouTcpClient(l))
      //{
      //  t.Connect();
      //  var c = new TenhouReceiver(t, "ID0160262B-SG8PcR2h", "M", 0);
      //  c.LogOn();
      //  Console.ReadKey();
      //  //c.RequestMatch();
      //  Console.ReadKey();
      //}
      //Console.ReadKey();

      var r = ClientFactory.CreateDummyClient();
      Console.ReadKey();
    }
  }
}