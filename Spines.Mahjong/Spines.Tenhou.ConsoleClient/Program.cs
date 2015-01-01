using System;
using Spines.Tenhou.Client;

namespace Spines.Tenhou.ConsoleClient
{
  class Program
  {
    static void Main()
    {
      using (var t = new TenhouTcpClient())
      {
        t.Connect();
        var c = new TenhouConnection(t);
        c.LogOn("ID0160262B-SG8PcR2h", "M", "0000");
        Console.ReadKey();
      }

      //var t = new DummyTenhouTcpClient();
      //var c = new TenhouConnection(t);
      //c.LogOn("ID0160262B-SG8PcR2h", "M", "0000");
      //Console.ReadKey();
    }
  }
}
