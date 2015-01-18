using System;
using Spines.Tenhou.Client;
using Spines.Utility;

namespace Spines.Tenhou.ConsoleClient
{
  class Program
  {
    static void Main()
    {
      //var l = new ConsoleLogger();
      //using (var t = new TenhouTcpClient(l))
      //{
      //  t.Connect();
      //  var c = new TenhouConnection(t, "ID0160262B-SG8PcR2h", "M", 0);
      //  c.LogOn();
      //  Console.ReadKey();
      //  //c.Join();
      //  Console.ReadKey();
      //}
      //Console.ReadKey();

      var l = new ConsoleLogger();
      var t = new DummyTenhouTcpClient(l);
      var i = new LogOnInformation("ID0160262B-SG8PcR2h", "M", 0);
      var c = new TenhouConnection(t, i);
      c.LogOn();
      c.Join();
      Console.ReadKey();
    }
  }
}
