using System;
using Spines.Tenhou.Client;
using Spines.Utility;

namespace Spines.Tenhou.ConsoleClient
{
  class Program
  {
    static void Main()
    {
      //using (var t = new TenhouTcpClient())
      //{
      //  t.Connect();
      //  var c = new TenhouConnection(t);
      //  c.LogOn("ID0160262B-SG8PcR2h", "M", "0000");
      //  Console.ReadKey();
      //}

      var l = new ConsoleLogger();
      var t = new DummyTenhouTcpClient(l);
      var c = new TenhouConnection(t);
      c.LogOn("ID0160262B-SG8PcR2h", "M", "0000");
      c.Join();
      Console.ReadKey();
    }
  }
}
