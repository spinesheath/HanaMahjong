using System;
using Spines.Tenhou.Client;
using Spines.Utility;

namespace Spines.Tenhou.ConsoleClient
{
  class Program
  {
    static void Main()
    {
      var l = new ConsoleLogger();
      using (var t = new TenhouTcpClient(l))
      {
        t.Connect();
        var c = new TenhouConnection(t);
        c.LogOn("ID0160262B-SG8PcR2h", "M", "0000");
        Console.ReadKey();
        c.Join();
        Console.ReadKey();
      }

      //var l = new ConsoleLogger();
      //var t = new DummyTenhouTcpClient(l);
      //var c = new TenhouConnection(t);
      //c.LogOn("ID0160262B-SG8PcR2h", "M", "0000");
      //c.Join();
      //Console.ReadKey();
    }
  }
}
